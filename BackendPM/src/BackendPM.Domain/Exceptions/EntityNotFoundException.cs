using System;

namespace BackendPM.Domain.Exceptions;

/// <summary>
/// 实体未找到异常
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="entityType">实体类型</param>
/// <param name="entityId">实体ID</param>
public class EntityNotFoundException(string entityType, object entityId) : DomainException($"实体 {entityType} 未找到，ID: {entityId}", "EntityNotFound")
{
    /// <summary>
    /// 实体类型
    /// </summary>
    public string EntityType { get; } = entityType;

    /// <summary>
    /// 实体ID
    /// </summary>
    public string EntityId { get; } = entityId?.ToString() ?? "null";
}