using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackendPM.Application.Behaviors.Logging;

/// <summary>
/// 日志记录行为管道 - 记录所有请求的执行过程
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var requestTypeName = request.GetType().Name;
        var requestGuid = Guid.NewGuid().ToString();
        
        // 记录请求开始
        _logger.LogInformation("开始处理请求 {RequestName} [{RequestGuid}]", requestTypeName, requestGuid);
        
        try
        {
            // 记录请求信息
            _logger.LogDebug("请求详情 {RequestName} [{RequestGuid}]: {@Request}", 
                requestTypeName, requestGuid, request);
                
            // 执行请求
            var response = await next();
            
            // 记录请求成功完成
            _logger.LogInformation("成功处理请求 {RequestName} [{RequestGuid}]", requestTypeName, requestGuid);
            
            return response;
        }
        catch (Exception ex)
        {
            // 记录请求异常
            _logger.LogError(ex, "处理请求 {RequestName} [{RequestGuid}] 时出错", 
                requestTypeName, requestGuid);
            throw;
        }
    }
}