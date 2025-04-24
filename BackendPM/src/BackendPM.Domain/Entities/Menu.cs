using BackendPM.Domain.Events;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Constants;

namespace BackendPM.Domain.Entities;

/// <summary>
/// 菜单实体
/// </summary>
public class Menu : EntityBase
{
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 菜单编码，用于系统识别
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// 菜单路径
    /// </summary>
    public string? Path { get; private set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    public string? Component { get; private set; }

    /// <summary>
    /// 父菜单ID，用于菜单层级结构
    /// </summary>
    public Guid? ParentMenuId { get; private set; }
    
    /// <summary>
    /// 父菜单，用于菜单层级结构
    /// </summary>
    public virtual Menu? ParentMenu { get; private set; }
    
    /// <summary>
    /// 子菜单列表
    /// </summary>
    public virtual ICollection<Menu> ChildMenus { get; private set; }

    /// <summary>
    /// 菜单关联的角色列表
    /// </summary>
    public virtual ICollection<RoleMenu> RoleMenus { get; private set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// 是否显示
    /// </summary>
    public bool Visible { get; private set; }

    /// <summary>
    /// 是否为系统菜单（系统菜单不能被删除）
    /// </summary>
    public bool IsSystem { get; private set; }

    // 无参构造函数用于EF Core
    private Menu()
    {
        Name = string.Empty;
        Code = string.Empty;
        RoleMenus = new List<RoleMenu>();
        ChildMenus = new List<Menu>();
    }

    /// <summary>
    /// 创建一个新菜单
    /// </summary>
    public Menu(
        string name, 
        string code, 
        string? path = null, 
        string? icon = null, 
        string? component = null,
        int sortOrder = 0, 
        bool visible = true, 
        bool isSystem = false, 
        Menu? parentMenu = null)
    {
        ValidateName(name);
        ValidateCode(code);

        Name = name;
        Code = code;
        Path = path;
        Icon = icon;
        Component = component;
        SortOrder = sortOrder;
        Visible = visible;
        IsSystem = isSystem;
        RoleMenus = new List<RoleMenu>();
        ChildMenus = new List<Menu>();
        
        // 设置父菜单
        if (parentMenu != null)
        {
            SetParentMenu(parentMenu);
        }
        
        // 添加菜单创建事件
        AddDomainEvent(new MenuCreatedEvent(this));
    }

    /// <summary>
    /// 更新菜单信息
    /// </summary>
    public void Update(
        string name, 
        string? path, 
        string? icon, 
        string? component,
        int sortOrder,
        bool visible)
    {
        ValidateName(name);

        Name = name;
        Path = path;
        Icon = icon;
        Component = component;
        SortOrder = sortOrder;
        Visible = visible;
        UpdateModificationTime();
        
        // 添加菜单更新事件
        AddDomainEvent(new MenuUpdatedEvent(this));
    }

    /// <summary>
    /// 设置父菜单，用于菜单层级结构
    /// </summary>
    public void SetParentMenu(Menu? parentMenu)
    {
        // 检测循环引用
        if (parentMenu != null && (parentMenu.Id == Id || HasMenuInHierarchy(parentMenu.Id)))
        {
            throw new BusinessRuleViolationException("不能将菜单设置为自身的子菜单或形成循环继承");
        }

        ParentMenu = parentMenu;
        ParentMenuId = parentMenu?.Id;
        UpdateModificationTime();
        
        // 触发事件
        AddDomainEvent(new MenuUpdatedEvent(this));
    }
    
    /// <summary>
    /// 检查指定菜单是否在当前菜单的继承链中
    /// </summary>
    /// <param name="menuId">要检查的菜单ID</param>
    /// <returns>是否在继承链中</returns>
    public bool HasMenuInHierarchy(Guid menuId)
    {
        // 检查当前菜单的所有子菜单
        foreach (var childMenu in ChildMenus)
        {
            if (childMenu.Id == menuId || childMenu.HasMenuInHierarchy(menuId))
                return true;
        }
        return false;
    }

    /// <summary>
    /// 设置菜单可见性
    /// </summary>
    public void SetVisibility(bool visible)
    {
        Visible = visible;
        UpdateModificationTime();
        
        // 触发事件
        AddDomainEvent(new MenuUpdatedEvent(this));
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("菜单名称不能为空", nameof(name));

        if (name.Length > 50)
            throw new ArgumentException("菜单名称不能超过50个字符", nameof(name));
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("菜单编码不能为空", nameof(code));

        if (code.Length > 50)
            throw new ArgumentException("菜单编码不能超过50个字符", nameof(code));
    }
}