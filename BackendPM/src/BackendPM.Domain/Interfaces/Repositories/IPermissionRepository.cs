using BackendPM.Domain.Entities;

namespace BackendPM.Domain.Interfaces.Repositories;

/// <summary>
/// 权限仓储接口
/// </summary>
public interface IPermissionRepository : IRepository<Permission>
{
    /// <summary>
    /// 根据权限代码查找权限
    /// </summary>
    /// <param name="code">权限代码</param>
    /// <returns>权限对象</returns>
    Task<Permission?> FindByCodeAsync(string code);
    
    /// <summary>
    /// 获取角色下的所有权限
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>权限列表</returns>
    Task<List<Permission>> GetRolePermissionsAsync(Guid roleId);
    
    /// <summary>
    /// 获取用户的所有权限（通过用户角色）
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>权限列表</returns>
    Task<List<Permission>> GetUserPermissionsAsync(Guid userId);
    
    /// <summary>
    /// 获取分页的权限列表
    /// </summary>
    /// <param name="pageIndex">页码，从1开始</param>
    /// <param name="pageSize">每页记录数</param>
    /// <param name="searchTerm">搜索关键词</param>
    /// <returns>权限分页列表</returns>
    Task<(List<Permission> Permissions, int TotalCount)> GetPagedListAsync(int pageIndex, int pageSize, string? searchTerm = null);
}