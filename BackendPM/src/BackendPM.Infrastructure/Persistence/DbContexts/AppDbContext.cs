using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces;
using BackendPM.Domain.Interfaces.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackendPM.Infrastructure.Persistence.DbContexts;

/// <summary>
/// 应用程序数据库上下文
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options,
                  IDomainEventDispatcher? domainEventDispatcher = null,
                  ILogger<AppDbContext>? logger = null) : DbContext(options)
{
    private readonly IDomainEventDispatcher? _domainEventDispatcher = domainEventDispatcher;
    private readonly ILogger<AppDbContext>? _logger = logger;

    /// <summary>
    /// 用户表
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// 角色表
    /// </summary>
    public DbSet<Role> Roles { get; set; } = null!;

    /// <summary>
    /// 权限表
    /// </summary>
    public DbSet<Permission> Permissions { get; set; } = null!;

    /// <summary>
    /// 部门表
    /// </summary>
    public DbSet<Department> Departments { get; set; } = null!;

    /// <summary>
    /// 菜单表
    /// </summary>
    public DbSet<Menu> Menus { get; set; } = null!;

    /// <summary>
    /// 用户角色关联表
    /// </summary>
    public DbSet<UserRole> UserRoles { get; set; } = null!;

    /// <summary>
    /// 角色权限关联表
    /// </summary>
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;

    /// <summary>
    /// 用户部门关联表
    /// </summary>
    public DbSet<UserDepartment> UserDepartments { get; set; } = null!;

    /// <summary>
    /// 角色菜单关联表
    /// </summary>
    public DbSet<RoleMenu> RoleMenus { get; set; } = null!;

    /// <summary>
    /// 刷新令牌表
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置用户实体
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).IsRequired();

            // 确保用户名和邮箱唯一
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // 配置角色实体
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(200);

            // 确保角色编码唯一
            entity.HasIndex(e => e.Code).IsUnique();
            
            // 配置角色继承关系
            entity.HasOne(r => r.ParentRole)
                .WithMany(r => r.ChildRoles)
                .HasForeignKey(r => r.ParentRoleId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); // 使用Restrict而非Cascade避免删除父角色时级联删除所有子角色
        });

        // 配置权限实体
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permissions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Code).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Group).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(200);

            // 确保权限编码唯一
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // 配置部门实体
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.SortOrder).IsRequired();
            entity.Property(e => e.IsSystem).IsRequired();

            // 确保部门编码唯一
            entity.HasIndex(e => e.Code).IsUnique();
            
            // 配置部门继承关系
            entity.HasOne(d => d.ParentDepartment)
                .WithMany(d => d.ChildDepartments)
                .HasForeignKey(d => d.ParentDepartmentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); // 使用Restrict而非Cascade避免删除父部门时级联删除所有子部门
        });

        // 配置菜单实体
        modelBuilder.Entity<Menu>(entity =>
        {
            entity.ToTable("Menus");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Path).HasMaxLength(200);
            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.Component).HasMaxLength(200);
            entity.Property(e => e.SortOrder).IsRequired();
            entity.Property(e => e.Visible).IsRequired();
            entity.Property(e => e.IsSystem).IsRequired();

            // 确保菜单编码唯一
            entity.HasIndex(e => e.Code).IsUnique();
            
            // 配置菜单继承关系
            entity.HasOne(m => m.ParentMenu)
                .WithMany(m => m.ChildMenus)
                .HasForeignKey(m => m.ParentMenuId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); // 使用Restrict而非Cascade避免删除父菜单时级联删除所有子菜单
        });

        // 配置用户角色关联
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(e => new { e.UserId, e.RoleId });

            // 配置与User的关系
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与Role的关系
            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置角色权限关联
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermissions");
            entity.HasKey(e => new { e.RoleId, e.PermissionId });

            // 配置与Role的关系
            entity.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与Permission的关系
            entity.HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置用户部门关联
        modelBuilder.Entity<UserDepartment>(entity =>
        {
            entity.ToTable("UserDepartments");
            entity.HasKey(e => new { e.UserId, e.DepartmentId });
            entity.Property(e => e.IsPrimary).IsRequired();

            // 配置与User的关系
            entity.HasOne(ud => ud.User)
                .WithMany(u => u.UserDepartments)
                .HasForeignKey(ud => ud.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与Department的关系
            entity.HasOne(ud => ud.Department)
                .WithMany(d => d.UserDepartments)
                .HasForeignKey(ud => ud.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置角色菜单关联
        modelBuilder.Entity<RoleMenu>(entity =>
        {
            entity.ToTable("RoleMenus");
            entity.HasKey(e => new { e.RoleId, e.MenuId });

            // 配置与Role的关系
            entity.HasOne(rm => rm.Role)
                .WithMany(r => r.RoleMenus)
                .HasForeignKey(rm => rm.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与Menu的关系
            entity.HasOne(rm => rm.Menu)
                .WithMany(m => m.RoleMenus)
                .HasForeignKey(rm => rm.MenuId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置刷新令牌实体
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired();

            // 配置与User的关系
            entity.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>
    /// 在保存更改前自动填充实体的创建和修改时间，并处理领域事件
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 处理实体的创建和修改时间
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.Entity.UpdateModificationTime();
                    break;

                case EntityState.Added:
                    // CreatedAt已在实体构造函数中设置
                    break;
            }
        }

        // 先保存所有更改
        var result = await base.SaveChangesAsync(cancellationToken);

        // 收集并处理所有领域事件
        if (_domainEventDispatcher != null)
        {
            var aggregateRoots = ChangeTracker.Entries<IAggregateRoot>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToList();

            if (aggregateRoots.Any())
            {
                _logger?.LogInformation("发现 {Count} 个聚合根有待处理的领域事件", aggregateRoots.Count);

                foreach (var aggregateRoot in aggregateRoots)
                {
                    var domainEvents = aggregateRoot.DomainEvents.ToList();
                    aggregateRoot.ClearDomainEvents();

                    if (domainEvents.Any())
                    {
                        _logger?.LogInformation("正在分发实体 {EntityType} (ID: {EntityId}) 的 {Count} 个领域事件",
                            aggregateRoot.GetType().Name,
                            (aggregateRoot as EntityBase)?.Id,
                            domainEvents.Count);

                        await _domainEventDispatcher.DispatchAsync(domainEvents);
                    }
                }
            }
        }

        return result;
    }
}