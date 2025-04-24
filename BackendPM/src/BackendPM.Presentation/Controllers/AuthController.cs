using BackendPM.Application.DTOs;
using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces.Repositories;
using BackendPM.Infrastructure.InfrastructureServices.Identity;
using BackendPM.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BackendPM.Presentation.Controllers;

/// <summary>
/// 用户认证控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthController> logger)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>登录结果，包含令牌信息</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        try
        {
            // 验证用户凭证
            var user = await _unitOfWork.Users.FindByUsernameOrEmailAsync(request.Username);
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "用户名或密码错误" });
            }

            if (!user.IsActive)
            {
                return Unauthorized(new { message = "账户已禁用，请联系管理员" });
            }

            // 获取用户角色和权限
            user = await _unitOfWork.Users.GetUserWithRolesAndPermissionsAsync(user.Id);
            
            // 确保用户及其角色不为空
            if (user == null || user.UserRoles == null)
            {
                return Unauthorized(new { message = "获取用户权限失败" });
            }
            
            // 提取用户权限
            var permissions = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Code))
                .Distinct()
                .ToList();

            // 生成访问令牌
            var accessToken = _jwtTokenService.GenerateAccessToken(user, permissions);
            
            // 生成刷新令牌
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryInDays);
            
            // 保存刷新令牌到数据库
            var refreshTokenEntity = new RefreshToken(user.Id, refreshToken, refreshTokenExpiry);
            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            
            // 清理该用户过期的刷新令牌，仅保留最近5个有效的令牌
            var userTokens = await _unitOfWork.RefreshTokens.GetAllByUserIdAsync(user.Id);
            var tokensToRemove = userTokens
                .Where(rt => rt.ExpiryTime < DateTime.UtcNow || rt.IsUsed || rt.IsRevoked)
                .ToList();

            if (userTokens.Count - tokensToRemove.Count > 5)
            {
                // 如果有效令牌数量超过5个，则只保留最近的5个
                var validTokens = userTokens
                    .Where(rt => !rt.IsUsed && !rt.IsRevoked && rt.ExpiryTime >= DateTime.UtcNow)
                    .OrderByDescending(rt => rt.CreatedAt)
                    .Skip(5)
                    .ToList();
                
                tokensToRemove.AddRange(validTokens);
            }

            if (tokensToRemove.Any())
            {
                foreach (var token in tokensToRemove)
                {
                    _unitOfWork.RefreshTokens.Delete(token);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            // 计算令牌过期时间戳
            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes).ToUnixTimeSeconds();

            // 构建响应
            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                Permissions = permissions
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用户登录时发生错误");
            return StatusCode(500, new { message = "服务器内部错误" });
        }
    }

    /// <summary>
    /// 刷新访问令牌
    /// </summary>
    /// <param name="request">刷新令牌请求</param>
    /// <returns>新的令牌信息</returns>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken(RefreshTokenRequestDto request)
    {
        try
        {
            // 从访问令牌中获取用户ID
            var userId = _jwtTokenService.GetUserIdFromToken(request.AccessToken);
            if (userId == null)
            {
                return BadRequest(new { message = "无效的访问令牌" });
            }

            // 验证刷新令牌
            var storedRefreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);
            if (storedRefreshToken == null || 
                storedRefreshToken.UserId != userId || 
                !storedRefreshToken.IsActive)
            {
                return BadRequest(new { message = "无效的刷新令牌" });
            }

            // 标记当前刷新令牌为已使用
            storedRefreshToken.MarkAsUsed();
            _unitOfWork.RefreshTokens.Update(storedRefreshToken);

            // 获取用户信息
            var user = await _unitOfWork.Users.GetUserWithRolesAndPermissionsAsync(userId.Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized(new { message = "用户不存在或已被禁用" });
            }

            // 确保用户角色不为空
            if (user.UserRoles == null)
            {
                return Unauthorized(new { message = "获取用户权限失败" });
            }

            // 提取用户权限
            var permissions = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Code))
                .Distinct()
                .ToList();

            // 生成新的访问令牌和刷新令牌
            var accessToken = _jwtTokenService.GenerateAccessToken(user, permissions);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryInDays);

            // 保存新的刷新令牌
            var refreshTokenEntity = new RefreshToken(user.Id, refreshToken, refreshTokenExpiry);
            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            // 计算令牌过期时间戳
            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes).ToUnixTimeSeconds();

            // 构建响应
            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                Permissions = permissions
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刷新令牌时发生错误");
            return StatusCode(500, new { message = "服务器内部错误" });
        }
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="request">修改密码请求</param>
    /// <returns>操作结果</returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto request)
    {
        try
        {
            // 获取当前登录用户ID
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "无效的用户会话" });
            }

            // 获取用户信息
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "用户不存在" });
            }

            // 验证当前密码
            if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                return BadRequest(new { message = "当前密码不正确" });
            }

            // 更新密码
            var newPasswordHash = HashPassword(request.NewPassword);
            user.UpdatePassword(newPasswordHash);
            _unitOfWork.Users.Update(user);

            // 撤销所有刷新令牌，强制用户重新登录
            var userRefreshTokens = await _unitOfWork.RefreshTokens.GetAllByUserIdAsync(userId);
            foreach (var token in userRefreshTokens)
            {
                token.Revoke();
                _unitOfWork.RefreshTokens.Update(token);
            }

            await _unitOfWork.SaveChangesAsync();

            return Ok(new { message = "密码已成功更新，请重新登录" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "修改密码时发生错误");
            return StatusCode(500, new { message = "服务器内部错误" });
        }
    }

    /// <summary>
    /// 注销登录
    /// </summary>
    /// <returns>操作结果</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // 获取当前登录用户ID
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Ok(new { message = "已注销" });
            }

            // 获取请求头中的令牌
            var authHeader = Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Ok(new { message = "已注销" });
            }

            // 获取用户的所有刷新令牌
            var refreshTokens = await _unitOfWork.RefreshTokens.GetAllByUserIdAsync(userId);

            // 撤销所有刷新令牌
            foreach (var refreshToken in refreshTokens)
            {
                refreshToken.Revoke();
                _unitOfWork.RefreshTokens.Update(refreshToken);
            }

            await _unitOfWork.SaveChangesAsync();

            return Ok(new { message = "已成功注销" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "注销登录时发生错误");
            return StatusCode(500, new { message = "服务器内部错误" });
        }
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns>当前用户信息</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        try
        {
            // 获取当前登录用户ID
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "无效的用户会话" });
            }

            // 获取用户信息
            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "用户不存在" });
            }

            // 构建用户信息响应
            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastModifiedAt = user.LastModifiedAt,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取当前用户信息时发生错误");
            return StatusCode(500, new { message = "服务器内部错误" });
        }
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    private static bool VerifyPassword(string password, string storedHash)
    {
        var computedHash = HashPassword(password);
        return storedHash == computedHash;
    }

    /// <summary>
    /// 对密码进行哈希处理
    /// </summary>
    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}