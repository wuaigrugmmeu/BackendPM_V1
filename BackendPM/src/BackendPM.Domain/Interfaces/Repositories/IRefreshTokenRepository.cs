using BackendPM.Domain.Entities;

namespace BackendPM.Domain.Interfaces.Repositories;

/// <summary>
/// 刷新令牌仓储接口
/// </summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    /// <summary>
    /// 根据令牌值查找刷新令牌
    /// </summary>
    /// <param name="token">令牌值</param>
    /// <returns>刷新令牌对象</returns>
    Task<RefreshToken?> FindByTokenAsync(string token);
    
    /// <summary>
    /// 根据令牌值获取刷新令牌（与FindByTokenAsync功能相同，兼容现有代码）
    /// </summary>
    /// <param name="token">令牌值</param>
    /// <returns>刷新令牌对象</returns>
    Task<RefreshToken?> GetByTokenAsync(string token);
    
    /// <summary>
    /// 获取用户的所有有效刷新令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>刷新令牌列表</returns>
    Task<List<RefreshToken>> GetUserActiveTokensAsync(Guid userId);
    
    /// <summary>
    /// 获取用户的所有刷新令牌（包含有效和无效的）
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>刷新令牌列表</returns>
    Task<List<RefreshToken>> GetAllByUserIdAsync(Guid userId);
    
    /// <summary>
    /// 使令牌失效
    /// </summary>
    /// <param name="token">令牌值</param>
    Task RevokeTokenAsync(string token);
    
    /// <summary>
    /// 使用户的所有令牌失效
    /// </summary>
    /// <param name="userId">用户ID</param>
    Task RevokeAllUserTokensAsync(Guid userId);
    
    /// <summary>
    /// 清理过期令牌
    /// </summary>
    Task CleanupExpiredTokensAsync();
}