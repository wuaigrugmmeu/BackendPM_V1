using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces.Repositories;
using BackendPM.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace BackendPM.Infrastructure.Persistence.Repositories;

/// <summary>
/// 用户仓储实现
/// </summary>
public class UserRepository(AppDbContext dbContext) : RepositoryBase<User>(dbContext), IUserRepository
{

    /// <summary>
    /// 根据用户名查找用户
    /// </summary>
    public async Task<User?> FindByUsernameAsync(string username)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    /// <summary>
    /// 根据电子邮件查找用户
    /// </summary>
    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// 根据用户名或电子邮件查找用户
    /// </summary>
    public async Task<User?> FindByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u =>
                u.Username == usernameOrEmail ||
                u.Email == usernameOrEmail);
    }

    /// <summary>
    /// 获取包含角色信息的用户
    /// </summary>
    public async Task<User?> GetWithRolesAsync(Guid userId)
    {
        return await _dbSet
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    /// <summary>
    /// 获取包含角色和权限信息的用户
    /// </summary>
    public async Task<User?> GetUserWithRolesAndPermissionsAsync(Guid userId)
    {
        // 此方法与GetWithRolesAsync功能相同，保留此方法名称以兼容现有代码
        return await GetWithRolesAsync(userId);
    }

    /// <summary>
    /// 获取包含角色信息的用户（根据ID）
    /// </summary>
    public async Task<User?> GetByIdWithRolesAsync(Guid userId)
    {
        // 此方法与GetWithRolesAsync功能相同，保留此方法名称以兼容现有代码
        return await GetWithRolesAsync(userId);
    }

    /// <summary>
    /// 获取所有包含角色信息的用户
    /// </summary>
    public async Task<List<User>> GetAllWithRolesAsync()
    {
        return await _dbSet
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .ToListAsync();
    }

    /// <summary>
    /// 获取分页的用户列表
    /// </summary>
    public async Task<(List<User> Users, int TotalCount)> GetPagedListAsync(int pageIndex, int pageSize, string? searchTerm = null)
    {
        IQueryable<User> query = _dbSet;

        // 应用搜索条件
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.Username.Contains(searchTerm) ||
                u.Email.Contains(searchTerm) ||
                (u.FullName != null && u.FullName.Contains(searchTerm))
            );
        }

        // 获取总记录数
        int totalCount = await query.CountAsync();

        // 应用分页
        var users = await query
            .OrderBy(u => u.Username)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .ToListAsync();

        return (users, totalCount);
    }
}