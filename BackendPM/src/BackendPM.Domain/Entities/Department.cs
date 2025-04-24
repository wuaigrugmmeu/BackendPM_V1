using BackendPM.Domain.Events;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Constants;

namespace BackendPM.Domain.Entities;

/// <summary>
/// 部门实体
/// </summary>
public class Department : EntityBase
{
    /// <summary>
    /// 部门名称
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 部门编码，用于系统识别
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 父部门ID，用于部门层级结构
    /// </summary>
    public Guid? ParentDepartmentId { get; private set; }
    
    /// <summary>
    /// 父部门，用于部门层级结构
    /// </summary>
    public virtual Department? ParentDepartment { get; private set; }
    
    /// <summary>
    /// 子部门列表
    /// </summary>
    public virtual ICollection<Department> ChildDepartments { get; private set; }

    /// <summary>
    /// 部门下的用户列表
    /// </summary>
    public virtual ICollection<UserDepartment> UserDepartments { get; private set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// 是否为系统部门（系统部门不能被删除）
    /// </summary>
    public bool IsSystem { get; private set; }

    // 无参构造函数用于EF Core
    private Department()
    {
        Name = string.Empty;
        Code = string.Empty;
        UserDepartments = new List<UserDepartment>();
        ChildDepartments = new List<Department>();
    }

    /// <summary>
    /// 创建一个新部门
    /// </summary>
    public Department(string name, string code, string? description = null, int sortOrder = 0, bool isSystem = false, Department? parentDepartment = null)
    {
        ValidateName(name);
        ValidateCode(code);

        Name = name;
        Code = code;
        Description = description;
        SortOrder = sortOrder;
        IsSystem = isSystem;
        UserDepartments = new List<UserDepartment>();
        ChildDepartments = new List<Department>();
        
        // 设置父部门
        if (parentDepartment != null)
        {
            SetParentDepartment(parentDepartment);
        }
        
        // 添加部门创建事件
        AddDomainEvent(new DepartmentCreatedEvent(this));
    }

    /// <summary>
    /// 更新部门信息
    /// </summary>
    public void Update(string name, string? description, int sortOrder)
    {
        ValidateName(name);

        Name = name;
        Description = description;
        SortOrder = sortOrder;
        UpdateModificationTime();
        
        // 添加部门更新事件
        AddDomainEvent(new DepartmentUpdatedEvent(this));
    }

    /// <summary>
    /// 设置父部门，用于部门层级结构
    /// </summary>
    public void SetParentDepartment(Department? parentDepartment)
    {
        // 检测循环引用
        if (parentDepartment != null && (parentDepartment.Id == Id || HasDepartmentInHierarchy(parentDepartment.Id)))
        {
            throw new BusinessRuleViolationException("不能将部门设置为自身的子部门或形成循环继承");
        }

        ParentDepartment = parentDepartment;
        ParentDepartmentId = parentDepartment?.Id;
        UpdateModificationTime();
        
        // 触发事件
        AddDomainEvent(new DepartmentUpdatedEvent(this));
    }
    
    /// <summary>
    /// 检查指定部门是否在当前部门的继承链中
    /// </summary>
    /// <param name="departmentId">要检查的部门ID</param>
    /// <returns>是否在继承链中</returns>
    public bool HasDepartmentInHierarchy(Guid departmentId)
    {
        // 检查当前部门的所有子部门
        foreach (var childDepartment in ChildDepartments)
        {
            if (childDepartment.Id == departmentId || childDepartment.HasDepartmentInHierarchy(departmentId))
                return true;
        }
        return false;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("部门名称不能为空", nameof(name));

        if (name.Length > 50)
            throw new ArgumentException("部门名称不能超过50个字符", nameof(name));
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("部门编码不能为空", nameof(code));

        if (code.Length > 50)
            throw new ArgumentException("部门编码不能超过50个字符", nameof(code));
    }
}