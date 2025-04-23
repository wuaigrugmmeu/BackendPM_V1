using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackendPM.Presentation.Models;

/// <summary>
/// API错误响应模型
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// 错误代码
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = "InternalServerError";
    
    /// <summary>
    /// 错误消息
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = "发生了内部服务器错误";
    
    /// <summary>
    /// 详细错误信息（仅在开发环境下返回）
    /// </summary>
    [JsonPropertyName("details")]
    public string? Details { get; set; }
    
    /// <summary>
    /// 错误发生时间
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 请求路径
    /// </summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }
    
    /// <summary>
    /// 请求ID（用于跟踪和日志关联）
    /// </summary>
    [JsonPropertyName("requestId")]
    public string? RequestId { get; set; }
    
    /// <summary>
    /// 验证错误信息列表
    /// </summary>
    [JsonPropertyName("errors")]
    public Dictionary<string, List<string>>? ValidationErrors { get; set; }
}