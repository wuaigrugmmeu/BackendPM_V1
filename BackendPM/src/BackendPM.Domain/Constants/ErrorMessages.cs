using System;

namespace BackendPM.Domain.Constants;

/// <summary>
/// 系统错误消息常量
/// </summary>
public static class ErrorMessages
{
    #region 实体类型名称
    public static class EntityNames
    {
        public const string UserType = "用户";
        public const string RoleType = "角色";
        public const string PermissionType = "权限";
    }
    #endregion

    #region 通用错误
    public static class Common
    {
        public const string EntityNotFound = "{0}不存在 (ID: {1})";
        public const string UnauthorizedOperation = "未授权的操作";
        public const string InvalidOperation = "无效的操作";
        public const string ConfigurationMissing = "{0}配置缺失";
    }
    #endregion

    #region 用户相关错误
    public static class User
    {
        public const string InvalidCredentials = "用户名或密码错误";
        public const string AccountLocked = "账户已被锁定";
        public const string AccountDisabled = "账户已被禁用";
        public const string EmailAlreadyUsed = "电子邮件 '{0}' 已被其他用户使用";
        public const string UsernameAlreadyUsed = "用户名 '{0}' 已被使用";
    }
    #endregion

    #region 角色相关错误
    public static class Role
    {
        public const string SystemRoleModificationForbidden = "系统角色不允许修改";
        public const string SystemRoleRequiresPermissions = "系统角色不能设置为无权限";
        public const string SystemRoleDeletionForbidden = "系统角色不允许删除";
        public const string RoleInUse = "无法删除角色 '{0}'，因为它已分配给{1}个用户";
        public const string CodeAlreadyExists = "角色编码 '{0}' 已存在";
        public const string NameRequired = "角色名称不能为空";
        public const string NameTooLong = "角色名称过长";
        public const string CodeRequired = "角色编码不能为空";
        public const string CodeTooLong = "角色编码过长";
    }
    #endregion

    #region 权限相关错误
    public static class Permission
    {
        public const string SystemPermissionModificationForbidden = "系统内置权限不允许修改基本信息";
        public const string NameRequired = "权限名称不能为空";
        public const string NameTooLong = "权限名称过长";
        public const string CodeRequired = "权限编码不能为空";
        public const string CodeTooLong = "权限编码过长";
        public const string GroupRequired = "权限分组不能为空";
        public const string GroupTooLong = "权限分组名称过长";
    }
    #endregion

    #region 认证相关错误
    public static class Auth
    {
        public const string JwtSecretKeyMissing = "JWT Secret Key is not configured";
        public const string InvalidToken = "无效的令牌";
        public const string TokenExpired = "令牌已过期";
        public const string RefreshTokenInvalid = "刷新令牌无效";
    }
    #endregion

    #region 系统错误
    public static class System
    {
        public const string RepositoryCreationFailed = "无法创建实体 {0} 的仓储";
    }
    #endregion
}