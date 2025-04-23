using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackendPM.Application.Behaviors.Transaction;

/// <summary>
/// 事务行为管道 - 为所有命令提供事务支持
/// </summary>
/// <typeparam name="TRequest">请求类型，必须是IRequest</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        // 如果请求不是命令（如查询），则直接执行
        if (IsQuery())
        {
            return await next();
        }
        
        var requestTypeName = request.GetType().Name;
        var requestGuid = Guid.NewGuid().ToString();

        try
        {
            _logger.LogInformation("开始事务 {TransactionId} 处理 {RequestName}", requestGuid, requestTypeName);

            // 开始事务
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            // 执行请求处理
            var response = await next();
            
            // 提交事务
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("已提交事务 {TransactionId} 用于 {RequestName}", requestGuid, requestTypeName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "事务 {TransactionId} 处理 {RequestName} 时出错，正在回滚", 
                requestGuid, requestTypeName);
            
            // 回滚事务
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            _logger.LogInformation("已回滚事务 {TransactionId} 用于 {RequestName}", requestGuid, requestTypeName);
            
            throw;
        }
    }

    /// <summary>
    /// 判断请求是否为查询
    /// </summary>
    /// <returns>如果是查询则返回true，否则返回false</returns>
    private static bool IsQuery()
    {
        return typeof(TRequest).Name.EndsWith("Query");
    }
}