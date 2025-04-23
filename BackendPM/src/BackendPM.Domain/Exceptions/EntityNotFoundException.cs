using System;

namespace BackendPM.Domain.Exceptions;

/// <summary>
/// 实体未找到异常
/// </summary>
public class EntityNotFoundException : DomainException
{
    /// <summary>
    /// 实体类型
    /// </summary>
    public string EntityType { get; }
    
    /// <summary>
    /// 实体ID
    /// </summary>
    public string EntityId { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <param name="entityId">实体ID</param>
    public EntityNotFoundException(string entityType, object entityId)
        : base($"实体 {entityType} 未找到，ID: {entityId}", "EntityNotFound")
    {
        EntityType = entityType;
        EntityId = entityId?.ToString() ?? "null";
    }
}