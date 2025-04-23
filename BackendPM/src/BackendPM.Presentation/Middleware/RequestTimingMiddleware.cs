using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BackendPM.Presentation.Middleware;

/// <summary>
/// API请求耗时监控中间件
/// </summary>
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // 在响应头中添加请求开始时间
        context.Response.OnStarting(() => {
            var elapsed = stopwatch.ElapsedMilliseconds;
            context.Response.Headers["X-Response-Time"] = $"{elapsed}ms";
            
            // 记录日志，包含请求路径、HTTP方法和耗时
            var path = context.Request.Path;
            var method = context.Request.Method;
            
            // 根据耗时设置不同的日志级别
            if (elapsed > 500)
                _logger.LogWarning("慢请求: {Method} {Path} - 耗时: {Elapsed}ms", method, path, elapsed);
            else
                _logger.LogInformation("请求: {Method} {Path} - 耗时: {Elapsed}ms", method, path, elapsed);
            
            return Task.CompletedTask;
        });
        
        await _next(context);
    }
}