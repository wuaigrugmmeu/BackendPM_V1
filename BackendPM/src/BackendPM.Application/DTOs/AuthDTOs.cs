using System.ComponentModel.DataAnnotations;

namespace BackendPM.Application.DTOs;

/// <summary>
/// 登录请求DTO
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// 用户名或电子邮件
    /// </summary>
    [Required(ErrorMessage = "用户名/邮箱不能为空")]
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 登录响应DTO
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
    
    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
    
    /// <summary>
    /// 过期时间（UTC时间戳）
    /// </summary>
    public long ExpiresAt { get; set; }
    
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// 用户邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// 用户角色
    /// </summary>
    public List<string> Roles { get; set; } = new();
    
    /// <summary>
    /// 用户权限
    /// </summary>
    public List<string> Permissions { get; set; } = new();
}

/// <summary>
/// 令牌刷新请求DTO
/// </summary>
public class RefreshTokenRequestDto
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    [Required(ErrorMessage = "访问令牌不能为空")]
    public string AccessToken { get; set; } = string.Empty;
    
    /// <summary>
    /// 刷新令牌
    /// </summary>
    [Required(ErrorMessage = "刷新令牌不能为空")]
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// 修改密码请求DTO
/// </summary>
public class ChangePasswordRequestDto
{
    /// <summary>
    /// 当前密码
    /// </summary>
    [Required(ErrorMessage = "当前密码不能为空")]
    public string CurrentPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// 新密码
    /// </summary>
    [Required(ErrorMessage = "新密码不能为空")]
    [MinLength(6, ErrorMessage = "新密码长度不能少于6个字符")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$", 
        ErrorMessage = "新密码必须包含大小写字母、数字和特殊字符")]
    public string NewPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// 确认新密码
    /// </summary>
    [Required(ErrorMessage = "确认密码不能为空")]
    [Compare("NewPassword", ErrorMessage = "两次输入的密码不一致")]
    public string ConfirmPassword { get; set; } = string.Empty;
}