using BackendPM.Domain.Entities;

namespace BackendPM.Domain.Interfaces.Repositories;

/// <summary>
/// 菜单仓储接口
/// </summary>
public interface IMenuRepository : IRepository<Menu>
{
    /// <summary>
    /// 获取所有菜单（包括层级关系）
    /// </summary>
    Task<List<Menu>> GetAllWithHierarchyAsync();
    
    /// <summary>
    /// 根据编码获取菜单
    /// </summary>
    Task<Menu?> GetByCodeAsync(string code);
    
    /// <summary>
    /// 获取指定角色的所有菜单
    /// </summary>
    Task<List<Menu>> GetMenusForRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取指定用户的所有可访问菜单（通过用户角色）
    /// </summary>
    Task<List<Menu>> GetMenusForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 分页获取菜单列表
    /// </summary>
    Task<(List<Menu> Menus, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
}