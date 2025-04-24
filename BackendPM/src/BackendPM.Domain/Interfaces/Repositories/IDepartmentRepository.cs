using BackendPM.Domain.Entities;

namespace BackendPM.Domain.Interfaces.Repositories;

/// <summary>
/// 部门仓储接口
/// </summary>
public interface IDepartmentRepository : IRepository<Department>
{
    /// <summary>
    /// 获取所有部门（包括层级关系）
    /// </summary>
    Task<List<Department>> GetAllWithHierarchyAsync();
    
    /// <summary>
    /// 根据编码获取部门
    /// </summary>
    Task<Department?> GetByCodeAsync(string code);
    
    /// <summary>
    /// 获取指定用户的所有部门
    /// </summary>
    Task<List<Department>> GetDepartmentsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取指定用户的主部门
    /// </summary>
    Task<Department?> GetPrimaryDepartmentForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 分页获取部门列表
    /// </summary>
    Task<(List<Department> Departments, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
}