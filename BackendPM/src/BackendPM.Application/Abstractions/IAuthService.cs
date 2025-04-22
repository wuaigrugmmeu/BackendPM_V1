using BackendPM.Application.DTOs;

namespace BackendPM.Application.Abstractions;

/// <summary>
/// 认证服务接口
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 登录验证
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>认证响应</returns>
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    
    /// <summary>
    /// 刷新令牌
    /// </summary>
    /// <param name="request">刷新令牌请求</param>
    /// <returns>认证响应</returns>
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    
    /// <summary>
    /// 修改用户密码
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="request">修改密码请求</param>
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request);
    
    /// <summary>
    /// 登出系统
    /// </summary>
    /// <param name="userId">用户ID</param>
    Task LogoutAsync(Guid userId);
}