using BackendPM.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BackendPM.Infrastructure.InfrastructureServices.Identity;

/// <summary>
/// JWT令牌服务接口
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// 生成访问令牌
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="permissions">用户权限列表</param>
    /// <returns>JWT令牌</returns>
    string GenerateAccessToken(User user, IEnumerable<string> permissions);
    
    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    /// <returns>刷新令牌</returns>
    string GenerateRefreshToken();
    
    /// <summary>
    /// 从令牌中获取用户ID
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>用户ID</returns>
    Guid? GetUserIdFromToken(string token);
    
    /// <summary>
    /// 验证令牌
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>是否有效</returns>
    bool ValidateToken(string token);
}

/// <summary>
/// JWT令牌服务实现
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings, ILogger<JwtTokenService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// 生成访问令牌
    /// </summary>
    public string GenerateAccessToken(User user, IEnumerable<string> permissions)
    {
        try
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            // 创建身份声明
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim("username", user.Username)
            };
            
            // 添加角色声明
            foreach (var role in user.UserRoles.Select(ur => ur.Role.Name))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            // 添加权限声明
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }
            
            // 创建JWT令牌
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                signingCredentials: credentials
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成访问令牌时发生错误");
            throw;
        }
    }

    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// 验证令牌
    /// </summary>
    public bool ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;
            
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            
            return validatedToken != null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "验证令牌时发生错误");
            return false;
        }
    }

    /// <summary>
    /// 从令牌中获取用户ID
    /// </summary>
    public Guid? GetUserIdFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;
            
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            
            // 仅验证签名，不验证过期时间
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false, // 刷新令牌时不验证访问令牌的过期时间
                ClockSkew = TimeSpan.Zero
            };
            
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
            var userIdClaim = principal.FindFirst("userId")?.Value;
            
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "从令牌获取用户ID时发生错误");
            return null;
        }
    }
}