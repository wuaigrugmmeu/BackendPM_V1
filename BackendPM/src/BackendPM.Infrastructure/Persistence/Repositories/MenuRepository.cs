using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces.Repositories;
using BackendPM.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace BackendPM.Infrastructure.Persistence.Repositories;

/// <summary>
/// 菜单仓储实现
/// </summary>
public class MenuRepository(AppDbContext dbContext) : RepositoryBase<Menu>(dbContext), IMenuRepository
{
    /// <summary>
    /// 获取所有菜单（包括层级关系）
    /// </summary>
    public async Task<List<Menu>> GetAllWithHierarchyAsync()
    {
        // 先获取所有菜单
        var menus = await _dbSet
            .Include(m => m.ChildMenus)
            .Include(m => m.RoleMenus)
                .ThenInclude(rm => rm.Role)
            .ToListAsync();

        // 按层级结构组织
        return menus.Where(m => m.ParentMenuId == null).ToList();
    }

    /// <summary>
    /// 根据编码获取菜单
    /// </summary>
    public async Task<Menu?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .FirstOrDefaultAsync(m => m.Code == code);
    }

    /// <summary>
    /// 获取指定角色的所有菜单
    /// </summary>
    public async Task<List<Menu>> GetMenusForRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoleMenus
            .Where(rm => rm.RoleId == roleId)
            .Include(rm => rm.Menu)
            .Select(rm => rm.Menu)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 获取指定用户的所有可访问菜单（通过用户角色）
    /// </summary>
    public async Task<List<Menu>> GetMenusForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // 获取用户所有角色ID
        var roleIds = await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);
        
        if (roleIds.Count == 0)
        {
            return new List<Menu>();
        }

        // 获取这些角色可以访问的所有菜单，去重
        var menus = await _dbContext.RoleMenus
            .Where(rm => roleIds.Contains(rm.RoleId))
            .Include(rm => rm.Menu)
            .Select(rm => rm.Menu)
            .Distinct()
            .ToListAsync(cancellationToken);

        // 仅返回可见的菜单
        return menus.Where(m => m.Visible).ToList();
    }

    /// <summary>
    /// 分页获取菜单列表
    /// </summary>
    public async Task<(List<Menu> Menus, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Menu> query = _dbSet;

        // 应用搜索条件
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(m => 
                m.Name.Contains(searchTerm) || 
                m.Code.Contains(searchTerm) ||
                (m.Path != null && m.Path.Contains(searchTerm)));
        }

        // 获取总数
        var totalCount = await query.CountAsync(cancellationToken);

        // 应用分页并获取结果
        var menus = await query
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.Name)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Include(m => m.ParentMenu)
            .ToListAsync(cancellationToken);

        return (menus, totalCount);
    }
}