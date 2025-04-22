using BackendPM.Domain.Interfaces;
using BackendPM.Domain.Interfaces.Events;

namespace BackendPM.Domain.Entities;

/// <summary>
/// 实体基类，所有实体都应该继承此类
/// </summary>
public abstract class EntityBase: IAggregateRoot
{
    /// <summary>
    /// 实体唯一标识
    /// </summary>
    public Guid Id { get; protected set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; protected set; }
    
    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime? LastModifiedAt { get; protected set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => throw new NotImplementedException();

    protected EntityBase()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// 更新实体的最后修改时间
    /// </summary>
    public void UpdateModificationTime()
    {
        LastModifiedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// 检查实体是否相等（基于Id）
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// 获取实体的哈希码（基于Id）
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        throw new NotImplementedException();
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        throw new NotImplementedException();
    }

    public void ClearDomainEvents()
    {
        throw new NotImplementedException();
    }

    public static bool operator ==(EntityBase? left, EntityBase? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(EntityBase? left, EntityBase? right)
    {
        return !(left == right);
    }
}