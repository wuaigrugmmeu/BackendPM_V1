using BackendPM.Domain.Constants;
using BackendPM.Domain.Events;
using BackendPM.Domain.Exceptions;

namespace BackendPM.Domain.Entities;

/// <summary>
/// 角色实体
/// </summary>
public class Role : EntityBase
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 角色编码，用于系统识别
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 是否为系统角色（系统角色不能被删除）
    /// </summary>
    public bool IsSystem { get; private set; }

    /// <summary>
    /// 父角色ID，用于角色继承
    /// </summary>
    public Guid? ParentRoleId { get; private set; }
    
    /// <summary>
    /// 父角色，用于角色继承
    /// </summary>
    public virtual Role? ParentRole { get; private set; }
    
    /// <summary>
    /// 子角色列表，继承该角色的所有权限
    /// </summary>
    public virtual ICollection<Role> ChildRoles { get; private set; }

    /// <summary>
    /// 角色关联的用户列表
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; private set; }

    /// <summary>
    /// 角色拥有的权限列表
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; private set; }

    /// <summary>
    /// 角色关联的菜单列表
    /// </summary>
    public virtual ICollection<RoleMenu> RoleMenus { get; private set; }

    // 无参构造函数用于EF Core
    private Role()
    {
        Name = string.Empty;
        Code = string.Empty;
        UserRoles = new List<UserRole>();
        RolePermissions = new List<RolePermission>();
        RoleMenus = new List<RoleMenu>();
        ChildRoles = new List<Role>();
    }

    /// <summary>
    /// 创建一个新角色
    /// </summary>
    public Role(string name, string code, string? description = null, bool isSystem = false, Role? parentRole = null)
    {
        ValidateName(name);
        ValidateCode(code);

        Name = name;
        Code = code;
        Description = description;
        IsSystem = isSystem;
        UserRoles = new List<UserRole>();
        RolePermissions = new List<RolePermission>();
        RoleMenus = new List<RoleMenu>();
        ChildRoles = new List<Role>();
        
        // 设置父角色
        if (parentRole != null)
        {
            SetParentRole(parentRole);
        }
        
        // 添加角色创建事件
        AddDomainEvent(new RoleCreatedEvent(this));
    }

    /// <summary>
    /// 更新角色信息
    /// </summary>
    public void Update(string name, string? description)
    {
        ValidateName(name);

        Name = name;
        Description = description;
        UpdateModificationTime();
        
        // 添加角色更新事件
        AddDomainEvent(new RoleUpdatedEvent(this));
    }

    /// <summary>
    /// 添加权限到角色
    /// </summary>
    public void AddPermission(Permission permission)
    {
        if (RolePermissions.Any(rp => rp.PermissionId == permission.Id))
            return;

        RolePermissions.Add(new RolePermission(this, permission));
        UpdateModificationTime();
        
        // 添加权限变更事件
        AddDomainEvent(new RolePermissionAddedEvent(this, permission));
    }

    /// <summary>
    /// 从角色中移除权限
    /// </summary>
    public void RemovePermission(Permission permission)
    {
        var rolePermission = RolePermissions.FirstOrDefault(rp => rp.PermissionId == permission.Id);
        if (rolePermission != null)
        {
            RolePermissions.Remove(rolePermission);
            UpdateModificationTime();
            
            // 添加权限移除事件
            AddDomainEvent(new RolePermissionRemovedEvent(this, permission));
        }
    }

    /// <summary>
    /// 批量设置角色权限
    /// </summary>
    /// <param name="permissions">要设置的权限列表</param>
    /// <remarks>
    /// 此方法会清除当前角色的所有权限，然后添加指定的权限
    /// </remarks>
    public void SetPermissions(IEnumerable<Permission> permissions)
    {
        // 如果是系统角色且权限列表为空，则抛出异常
        if (IsSystem && (permissions == null || !permissions.Any()))
        {
            throw new BusinessRuleViolationException(ErrorMessages.Role.SystemRoleRequiresPermissions);
        }
        
        // 保存当前权限列表，用于计算变更
        var currentPermissions = RolePermissions.Select(rp => rp.Permission).ToList();
        var newPermissions = permissions?.ToList() ?? new List<Permission>();
        
        // 计算添加和移除的权限
        var addedPermissions = newPermissions.Where(p => !currentPermissions.Any(cp => cp.Id == p.Id)).ToList();
        var removedPermissions = currentPermissions.Where(p => !newPermissions.Any(np => np.Id == p.Id)).ToList();

        // 清除当前所有权限
        RolePermissions.Clear();

        // 添加新的权限
        if (permissions != null)
        {
            foreach (var permission in permissions)
            {
                RolePermissions.Add(new RolePermission(this, permission));
            }
        }

        UpdateModificationTime();
        
        // 添加批量权限变更事件
        if (addedPermissions.Any() || removedPermissions.Any())
        {
            AddDomainEvent(new RolePermissionsBulkChangedEvent(this, addedPermissions, removedPermissions));
        }
    }

    /// <summary>
    /// 批量添加权限
    /// </summary>
    /// <param name="permissions">要添加的权限列表</param>
    public void AddPermissions(IEnumerable<Permission> permissions)
    {
        if (permissions == null)
            return;

        bool changed = false;
        foreach (var permission in permissions)
        {
            if (!RolePermissions.Any(rp => rp.PermissionId == permission.Id))
            {
                RolePermissions.Add(new RolePermission(this, permission));
                changed = true;
            }
        }

        if (changed)
        {
            UpdateModificationTime();
        }
    }

    /// <summary>
    /// 检查角色是否拥有指定权限，包括从父角色继承的权限
    /// </summary>
    /// <param name="permissionCode">权限编码</param>
    /// <returns>是否拥有权限</returns>
    public bool HasPermission(string permissionCode)
    {
        // 检查自身权限
        if (RolePermissions.Any(rp => rp.Permission.Code == permissionCode))
            return true;
            
        // 检查父角色权限（递归向上查找）
        if (ParentRole != null)
            return ParentRole.HasPermission(permissionCode);
            
        return false;
    }

    /// <summary>
    /// 检查角色是否拥有指定资源的访问权限，包括从父角色继承的权限
    /// </summary>
    /// <param name="resourcePath">资源路径</param>
    /// <param name="httpMethod">HTTP方法</param>
    /// <returns>是否拥有权限</returns>
    public bool HasPermissionForResource(string resourcePath, string httpMethod)
    {
        // 检查自身权限
        bool hasOwnPermission = CheckOwnPermissionForResource(resourcePath, httpMethod);
        if (hasOwnPermission)
            return true;
            
        // 检查父角色权限
        if (ParentRole != null)
            return ParentRole.HasPermissionForResource(resourcePath, httpMethod);
            
        return false;
    }
    
    /// <summary>
    /// 检查角色自身是否拥有指定资源的访问权限（不考虑继承）
    /// </summary>
    private bool CheckOwnPermissionForResource(string resourcePath, string httpMethod)
    {
        // 检查是否有精确匹配的权限
        var hasExactPermission = RolePermissions.Any(rp => 
            rp.Permission.ResourcePath == resourcePath && 
            (rp.Permission.HttpMethod == httpMethod || string.IsNullOrEmpty(rp.Permission.HttpMethod)));

        if (hasExactPermission)
            return true;

        // 检查是否有通配符权限
        foreach (var rolePermission in RolePermissions)
        {
            var permission = rolePermission.Permission;
            
            // 跳过非API类型权限
            if (permission.ResourceType != "API")
                continue;
                
            // 检查HTTP方法是否匹配或权限中未指定HTTP方法
            bool methodMatches = string.IsNullOrEmpty(permission.HttpMethod) || 
                                permission.HttpMethod == httpMethod || 
                                permission.HttpMethod == "*";
                
            if (!methodMatches)
                continue;
                
            // 检查路径是否匹配
            if (!string.IsNullOrEmpty(permission.ResourcePath) && IsPathMatch(permission.ResourcePath, resourcePath))
                return true;
        }
        
        return false;
    }

    /// <summary>
    /// 检查资源路径是否匹配权限中的路径模式
    /// </summary>
    /// <param name="pattern">权限中的路径模式，可包含通配符</param>
    /// <param name="path">请求的资源路径</param>
    /// <returns>是否匹配</returns>
    private bool IsPathMatch(string pattern, string path)
    {
        // 完全匹配
        if (pattern == path)
            return true;
            
        // 通配符 * 匹配
        if (pattern.EndsWith("/*") && path.StartsWith(pattern.TrimEnd('*')))
            return true;
            
        // 通配符 ** 匹配（递归子路径）
        if (pattern.EndsWith("/**") && path.StartsWith(pattern.TrimEnd('*', '/')))
            return true;
            
        return false;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(ErrorMessages.Role.NameRequired, nameof(name));

        if (name.Length > 50)
            throw new ArgumentException(ErrorMessages.Role.NameTooLong, nameof(name));
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(ErrorMessages.Role.CodeRequired, nameof(code));

        if (code.Length > 50)
            throw new ArgumentException(ErrorMessages.Role.CodeTooLong, nameof(code));
    }

    /// <summary>
    /// 设置父角色，用于角色继承
    /// </summary>
    public void SetParentRole(Role? parentRole)
    {
        // 检测循环引用
        if (parentRole != null && (parentRole.Id == Id || HasRoleInHierarchy(parentRole.Id)))
        {
            throw new BusinessRuleViolationException("不能将角色设置为自身的子角色或形成循环继承");
        }

        ParentRole = parentRole;
        ParentRoleId = parentRole?.Id;
        UpdateModificationTime();
        
        // 触发事件
        AddDomainEvent(new RoleUpdatedEvent(this));
    }
    
    /// <summary>
    /// 检查指定角色是否在当前角色的继承链中
    /// </summary>
    /// <param name="roleId">要检查的角色ID</param>
    /// <returns>是否在继承链中</returns>
    public bool HasRoleInHierarchy(Guid roleId)
    {
        // 检查当前角色的所有子角色
        foreach (var childRole in ChildRoles)
        {
            if (childRole.Id == roleId || childRole.HasRoleInHierarchy(roleId))
                return true;
        }
        return false;
    }

    /// <summary>
    /// 添加菜单到角色
    /// </summary>
    public void AddMenu(Menu menu)
    {
        if (RoleMenus.Any(rm => rm.MenuId == menu.Id))
            return;

        RoleMenus.Add(new RoleMenu(this, menu));
        UpdateModificationTime();
        
        // 添加菜单变更事件
        AddDomainEvent(new RoleMenuAddedEvent(this, menu));
    }

    /// <summary>
    /// 从角色中移除菜单
    /// </summary>
    public void RemoveMenu(Menu menu)
    {
        var roleMenu = RoleMenus.FirstOrDefault(rm => rm.MenuId == menu.Id);
        if (roleMenu != null)
        {
            RoleMenus.Remove(roleMenu);
            UpdateModificationTime();
            
            // 添加菜单移除事件
            AddDomainEvent(new RoleMenuRemovedEvent(this, menu));
        }
    }

    /// <summary>
    /// 批量设置角色菜单
    /// </summary>
    /// <param name="menus">要设置的菜单列表</param>
    public void SetMenus(IEnumerable<Menu> menus)
    {
        // 保存当前菜单列表，用于计算变更
        var currentMenus = RoleMenus.Select(rm => rm.Menu).ToList();
        var newMenus = menus?.ToList() ?? new List<Menu>();
        
        // 计算添加和移除的菜单
        var addedMenus = newMenus.Where(m => !currentMenus.Any(cm => cm.Id == m.Id)).ToList();
        var removedMenus = currentMenus.Where(m => !newMenus.Any(nm => nm.Id == m.Id)).ToList();

        // 清除当前所有菜单
        RoleMenus.Clear();

        // 添加新的菜单
        if (menus != null)
        {
            foreach (var menu in menus)
            {
                RoleMenus.Add(new RoleMenu(this, menu));
            }
        }

        UpdateModificationTime();
        
        // 添加批量菜单变更事件
        if (addedMenus.Any() || removedMenus.Any())
        {
            AddDomainEvent(new RoleMenusBulkChangedEvent(this, addedMenus, removedMenus));
        }
    }

    /// <summary>
    /// 检查角色是否拥有指定菜单，包括从父角色继承的菜单
    /// </summary>
    /// <param name="menuCode">菜单编码</param>
    /// <returns>是否拥有菜单</returns>
    public bool HasMenu(string menuCode)
    {
        // 检查自身菜单
        if (RoleMenus.Any(rm => rm.Menu.Code == menuCode))
            return true;
            
        // 检查父角色菜单（递归向上查找）
        if (ParentRole != null)
            return ParentRole.HasMenu(menuCode);
            
        return false;
    }
}