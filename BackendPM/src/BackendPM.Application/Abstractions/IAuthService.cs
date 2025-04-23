using System.Threading.Tasks;
using BackendPM.Application.DTOs;

namespace BackendPM.Application.Abstractions;

/// <summary>
/// 认证服务接口
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 登录并获取令牌
    /// </summary>
    /// <param name="loginRequest">登录请求</param>
    /// <returns>登录响应，包含访问令牌和刷新令牌</returns>
    Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
    
    /// <summary>
    /// 刷新令牌
    /// </summary>
    /// <param name="refreshRequest">刷新令牌请求</param>
    /// <returns>刷新令牌响应，包含新的访问令牌和刷新令牌</returns>
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshRequest);
}