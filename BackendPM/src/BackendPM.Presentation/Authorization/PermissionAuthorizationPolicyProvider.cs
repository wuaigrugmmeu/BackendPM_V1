using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace BackendPM.Presentation.Authorization;

/// <summary>
/// 权限授权策略提供程序
/// </summary>
public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private const string PERMISSION_POLICY_PREFIX = "Permission";

    /// <summary>
    /// 构造函数
    /// </summary>
    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    /// <summary>
    /// 获取授权策略
    /// </summary>
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // 如果不是权限策略，则使用默认策略提供程序
        if (!policyName.StartsWith(PERMISSION_POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
        {
            return await base.GetPolicyAsync(policyName);
        }

        // 从策略名称提取权限编码
        // 格式：Permission{权限编码}
        var permissionCode = policyName.Substring(PERMISSION_POLICY_PREFIX.Length);

        // 创建只包含单个权限要求的授权策略
        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(permissionCode))
            .Build();

        return policy;
    }
}