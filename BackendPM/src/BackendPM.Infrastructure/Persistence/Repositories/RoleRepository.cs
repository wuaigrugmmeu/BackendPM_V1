using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces.Repositories;
using BackendPM.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace BackendPM.Infrastructure.Persistence.Repositories;

/// <summary>
/// 角色仓储实现
/// </summary>
public class RoleRepository : RepositoryBase<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Role?> FindByNameAsync(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Code == code, cancellationToken);
    }

    public async Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<(List<Role> Items, int TotalCount)> GetPagedAsync(
        int pageIndex, 
        int pageSize, 
        string? searchTerm = null, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<Role> query = _dbContext.Roles;

        // 应用搜索条件
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(r => 
                r.Name.Contains(searchTerm) || 
                r.Code.Contains(searchTerm) || 
                (r.Description != null && r.Description.Contains(searchTerm)));
        }

        // 获取总记录数
        int totalCount = await query.CountAsync(cancellationToken);

        // 应用分页
        var items = await query
            .OrderBy(r => r.Name)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<(List<Role> Roles, int TotalCount)> GetPagedListAsync(int pageIndex, int pageSize, string? searchTerm = null)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Role>> GetRolesForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Role>> GetUserRolesAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Role?> GetWithPermissionsAsync(Guid roleId)
    {
        throw new NotImplementedException();
    }
}