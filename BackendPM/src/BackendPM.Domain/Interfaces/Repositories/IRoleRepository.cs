using BackendPM.Domain.Entities;

namespace BackendPM.Domain.Interfaces.Repositories;

/// <summary>
/// 角色仓储接口
/// </summary>
public interface IRoleRepository : IRepository<Role>
{
    /// <summary>
    /// 根据角色名称查找角色
    /// </summary>
    /// <param name="name">角色名称</param>
    /// <returns>角色对象</returns>
    Task<Role?> FindByNameAsync(string name);
    
    /// <summary>
    /// 获取包含权限信息的角色
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>包含权限信息的角色对象</returns>
    Task<Role?> GetWithPermissionsAsync(Guid roleId);
    
    /// <summary>
    /// 获取用户的所有角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>角色列表</returns>
    Task<List<Role>> GetUserRolesAsync(Guid userId);
    
    /// <summary>
    /// 获取分页的角色列表
    /// </summary>
    /// <param name="pageIndex">页码，从1开始</param>
    /// <param name="pageSize">每页记录数</param>
    /// <param name="searchTerm">搜索关键词</param>
    /// <returns>角色分页列表</returns>
    Task<(List<Role> Roles, int TotalCount)> GetPagedListAsync(int pageIndex, int pageSize, string? searchTerm = null);
    
    /// <summary>
    /// 根据ID获取包含权限信息的角色
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>包含权限信息的角色对象</returns>
    Task<Role?> GetByIdWithPermissionsAsync(Guid roleId);
    
    /// <summary>
    /// 获取所有包含权限信息的角色
    /// </summary>
    /// <returns>包含权限信息的角色列表</returns>
    Task<List<Role>> GetAllWithPermissionsAsync();
}