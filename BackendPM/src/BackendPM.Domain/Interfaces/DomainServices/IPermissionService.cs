using System.Threading.Tasks;

namespace BackendPM.Domain.Interfaces.DomainServices;

/// <summary>
/// 权限服务接口
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// 检查用户是否有权限访问特定资源
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="resourcePath">资源路径</param>
    /// <param name="httpMethod">HTTP方法</param>
    /// <returns>是否有权限</returns>
    Task<bool> UserHasPermissionAsync(int userId, string? resourcePath, string? httpMethod);

    /// <summary>
    /// 检查用户是否拥有指定权限编码
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permissionCode">权限编码</param>
    /// <returns>是否有权限</returns>
    Task<bool> UserHasPermissionCodeAsync(int userId, string permissionCode);

    /// <summary>
    /// 获取用户的所有权限编码
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>权限编码列表</returns>
    Task<IEnumerable<string>> GetUserPermissionCodesAsync(int userId);

    /// <summary>
    /// 获取用户的所有权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>权限列表</returns>
    Task<IEnumerable<Entities.Permission>> GetUserPermissionsAsync(int userId);

    /// <summary>
    /// 清除用户权限缓存
    /// </summary>
    /// <param name="userId">用户ID</param>
    void ClearUserPermissionCache(int userId);
}