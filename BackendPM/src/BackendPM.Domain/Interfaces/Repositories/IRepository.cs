using System.Linq.Expressions;
using BackendPM.Domain.Interfaces;

namespace BackendPM.Domain.Interfaces.Repositories;

/// <summary>
/// 通用仓储接口
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public interface IRepository<TEntity> where TEntity : IAggregateRoot
{
    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns>实体对象</returns>
    Task<TEntity?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// 根据条件获取单个实体
    /// </summary>
    /// <param name="predicate">查询条件表达式</param>
    /// <returns>实体对象</returns>
    Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate);
    
    /// <summary>
    /// 根据条件查询实体列表
    /// </summary>
    /// <param name="predicate">查询条件表达式</param>
    /// <returns>实体列表</returns>
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    
    /// <summary>
    /// 获取IQueryable用于高级查询
    /// </summary>
    /// <returns>IQueryable</returns>
    IQueryable<TEntity> AsQueryable();
    
    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    Task AddAsync(TEntity entity);
    
    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    void Update(TEntity entity);
    
    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    void Delete(TEntity entity);
    
    /// <summary>
    /// 批量添加实体
    /// </summary>
    /// <param name="entities">实体列表</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    
    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entities">实体列表</param>
    void UpdateRange(IEnumerable<TEntity> entities);
    
    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <param name="entities">实体列表</param>
    void DeleteRange(IEnumerable<TEntity> entities);
    
    /// <summary>
    /// 统计符合条件的实体数量
    /// </summary>
    /// <param name="predicate">查询条件表达式</param>
    /// <returns>实体数量</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
    
    /// <summary>
    /// 判断是否存在符合条件的实体
    /// </summary>
    /// <param name="predicate">查询条件表达式</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
}