using BackendPM.Domain.Entities;

namespace BackendPM.Domain.Interfaces.Repositories;

/// <summary>
/// 用户仓储接口
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// 根据用户名查找用户
    /// </summary>
    /// <param name="username">用户名</param>
    /// <returns>用户对象</returns>
    Task<User?> FindByUsernameAsync(string username);

    /// <summary>
    /// 根据电子邮件查找用户
    /// </summary>
    /// <param name="email">电子邮件</param>
    /// <returns>用户对象</returns>
    Task<User?> FindByEmailAsync(string email);

    /// <summary>
    /// 根据用户名或电子邮件查找用户
    /// </summary>
    /// <param name="usernameOrEmail">用户名或电子邮件</param>
    /// <returns>用户对象</returns>
    Task<User?> FindByUsernameOrEmailAsync(string usernameOrEmail);

    /// <summary>
    /// 获取包含角色信息的用户
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>包含角色信息的用户对象</returns>
    Task<User?> GetWithRolesAsync(Guid userId);

    /// <summary>
    /// 获取包含角色和权限信息的用户
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>包含角色和权限信息的用户对象</returns>
    Task<User?> GetUserWithRolesAndPermissionsAsync(Guid userId);

    /// <summary>
    /// 获取包含角色信息的用户（根据ID）
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>包含角色信息的用户对象</returns>
    Task<User?> GetByIdWithRolesAsync(Guid userId);

    /// <summary>
    /// 获取分页的用户列表
    /// </summary>
    /// <param name="pageIndex">页码，从1开始</param>
    /// <param name="pageSize">每页记录数</param>
    /// <param name="searchTerm">搜索关键词</param>
    /// <returns>用户分页列表</returns>
    Task<(List<User> Users, int TotalCount)> GetPagedListAsync(int pageIndex, int pageSize, string? searchTerm = null);

    /// <summary>
    /// 获取所有包含角色信息的用户
    /// </summary>
    /// <returns>包含角色信息的用户列表</returns>
    Task<List<User>> GetAllWithRolesAsync();
}