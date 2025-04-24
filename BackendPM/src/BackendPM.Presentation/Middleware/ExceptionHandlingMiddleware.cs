using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using BackendPM.Domain.Exceptions;
using BackendPM.Presentation.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackendPM.Presentation.Middleware;

/// <summary>
/// 全局异常处理中间件
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="next">请求委托</param>
/// <param name="logger">日志记录器</param>
/// <param name="environment">环境信息</param>
public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;
    private readonly IHostEnvironment _environment = environment;

    /// <summary>
    /// 中间件执行方法
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>异步任务</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// 异常处理方法
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <param name="exception">异常</param>
    /// <returns>异步任务</returns>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // 记录异常详情
        _logger.LogError(exception, "请求处理过程中发生异常: {Path}", context.Request.Path);

        // 根据异常类型设置HTTP状态码和错误响应
        var statusCode = GetStatusCode(exception);
        var errorResponse = CreateErrorResponse(exception, context);

        // 设置响应格式和状态码
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        // 写入响应内容
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, jsonOptions));
    }

    /// <summary>
    /// 根据异常类型获取对应的HTTP状态码
    /// </summary>
    /// <param name="exception">异常</param>
    /// <returns>HTTP状态码</returns>
    private static HttpStatusCode GetStatusCode(Exception exception)
    {
        return exception switch
        {
            EntityNotFoundException => HttpStatusCode.NotFound, // 404
            AuthenticationException => HttpStatusCode.Unauthorized, // 401
            AuthorizationException => HttpStatusCode.Forbidden, // 403
            ValidationException => HttpStatusCode.BadRequest, // 400
            BusinessRuleViolationException => HttpStatusCode.UnprocessableEntity, // 422
            _ => HttpStatusCode.InternalServerError // 500
        };
    }

    /// <summary>
    /// 创建错误响应对象
    /// </summary>
    /// <param name="exception">异常</param>
    /// <param name="context">HTTP上下文</param>
    /// <returns>错误响应</returns>
    private ErrorResponse CreateErrorResponse(Exception exception, HttpContext context)
    {
        var errorResponse = new ErrorResponse
        {
            Path = context.Request.Path,
            RequestId = context.TraceIdentifier
        };

        // 根据异常类型设置错误代码和消息
        switch (exception)
        {
            case DomainException domainException:
                errorResponse.Code = domainException.Code;
                errorResponse.Message = domainException.Message;
                break;

            case ValidationException validationException:
                errorResponse.Code = "ValidationFailed";
                errorResponse.Message = "请求数据验证失败";
                errorResponse.ValidationErrors = [];

                // 处理验证错误
                foreach (var error in validationException.Errors)
                {
                    if (!errorResponse.ValidationErrors.TryGetValue(error.PropertyName, out List<string>? value))
                    {
                        value = ([]);
                        errorResponse.ValidationErrors[error.PropertyName] = value;
                    }

                    value.Add(error.ErrorMessage);
                }
                break;

            default:
                errorResponse.Code = "InternalServerError";
                errorResponse.Message = "服务器内部错误，请稍后再试";
                break;
        }

        // 在开发环境下添加详细错误信息
        if (_environment.IsDevelopment())
        {
            errorResponse.Details = exception.ToString();
        }

        return errorResponse;
    }
}