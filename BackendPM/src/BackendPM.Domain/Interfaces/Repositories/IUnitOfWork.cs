namespace BackendPM.Domain.Interfaces.Repositories;

/// <summary>
/// 工作单元接口，用于管理事务和仓储
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// 获取指定类型的仓储
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>仓储实例</returns>
    IRepository<TEntity> Repository<TEntity>() where TEntity : class, IAggregateRoot;

    /// <summary>
    /// 用户仓储
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// 角色仓储
    /// </summary>
    IRoleRepository Roles { get; }

    /// <summary>
    /// 权限仓储
    /// </summary>
    IPermissionRepository Permissions { get; }

    /// <summary>
    /// 刷新令牌仓储
    /// </summary>
    IRefreshTokenRepository RefreshTokens { get; }

    /// <summary>
    /// 保存所有变更
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 开始一个事务
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交当前事务
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 回滚当前事务
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}