using BackendPM.Domain.Interfaces;
using BackendPM.Domain.Interfaces.Repositories;
using BackendPM.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BackendPM.Infrastructure.Persistence.Repositories;

/// <summary>
/// 工作单元实现，用于管理事务和仓储
/// </summary>
public class UnitOfWork(
    AppDbContext dbContext,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository,
    IRefreshTokenRepository refreshTokenRepository) : IUnitOfWork
{
    private readonly AppDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private IDbContextTransaction? _currentTransaction;
    private readonly Dictionary<Type, object> _repositories = [];

    public IUserRepository Users { get; } = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    public IRoleRepository Roles { get; } = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    public IPermissionRepository Permissions { get; } = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
    public IRefreshTokenRepository RefreshTokens { get; } = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));

    /// <summary>
    /// 保存所有变更
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 开始一个事务
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            return;
        }

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// 提交当前事务
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);

            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    /// <summary>
    /// 回滚当前事务
    /// </summary>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _currentTransaction?.Dispose();
            _dbContext.Dispose();
        }
    }

    /// <summary>
    /// 获取指定实体类型的仓储
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>该实体类型的仓储实例</returns>
    public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IAggregateRoot
    {
        var type = typeof(TEntity);

        if (!_repositories.TryGetValue(type, out object? value))
        {
            var repositoryType = typeof(RepositoryBase<>).MakeGenericType(type);
            var repository = Activator.CreateInstance(repositoryType, _dbContext) ?? throw new InvalidOperationException($"无法创建实体 {type.Name} 的仓储");
            value = repository;
            _repositories.Add(type, value);
        }

        return (IRepository<TEntity>)value;
    }
}