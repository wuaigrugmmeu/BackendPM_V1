using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces.Repositories;
using BackendPM.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace BackendPM.Infrastructure.Persistence.Repositories;

/// <summary>
/// 部门仓储实现
/// </summary>
public class DepartmentRepository(AppDbContext dbContext) : RepositoryBase<Department>(dbContext), IDepartmentRepository
{
    /// <summary>
    /// 获取所有部门（包括层级关系）
    /// </summary>
    public async Task<List<Department>> GetAllWithHierarchyAsync()
    {
        // 先获取所有部门
        var departments = await _dbSet
            .Include(d => d.ChildDepartments)
            .Include(d => d.UserDepartments)
            .ToListAsync();

        // 按层级结构组织
        return departments.Where(d => d.ParentDepartmentId == null).ToList();
    }

    /// <summary>
    /// 根据编码获取部门
    /// </summary>
    public async Task<Department?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.Code == code);
    }

    /// <summary>
    /// 获取指定用户的所有部门
    /// </summary>
    public async Task<List<Department>> GetDepartmentsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserDepartments
            .Where(ud => ud.UserId == userId)
            .Include(ud => ud.Department)
            .Select(ud => ud.Department)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 获取指定用户的主部门
    /// </summary>
    public async Task<Department?> GetPrimaryDepartmentForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserDepartments
            .Where(ud => ud.UserId == userId && ud.IsPrimary)
            .Include(ud => ud.Department)
            .Select(ud => ud.Department)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 分页获取部门列表
    /// </summary>
    public async Task<(List<Department> Departments, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Department> query = _dbSet;

        // 应用搜索条件
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(d => 
                d.Name.Contains(searchTerm) || 
                d.Code.Contains(searchTerm) ||
                (d.Description != null && d.Description.Contains(searchTerm)));
        }

        // 获取总数
        var totalCount = await query.CountAsync(cancellationToken);

        // 应用分页并获取结果
        var departments = await query
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.Name)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Include(d => d.ParentDepartment)
            .ToListAsync(cancellationToken);

        return (departments, totalCount);
    }
}