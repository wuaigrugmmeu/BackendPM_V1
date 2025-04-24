using System;
using System.Threading;
using System.Threading.Tasks;
using BackendPM.Domain.Constants;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Commands.Users;

/// <summary>
/// 删除用户命令
/// </summary>
public class DeleteUserCommand(Guid userId) : BaseCommand
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; } = userId;
}

/// <summary>
/// 删除用户命令处理器
/// </summary>
public class DeleteUserCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteUserCommandHandler> logger) : IRequestHandler<DeleteUserCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<DeleteUserCommandHandler> _logger = logger;

    public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(command.UserId)
            ?? throw new EntityNotFoundException(ErrorMessages.EntityNames.UserType, command.UserId);

        // 执行删除操作
        _unitOfWork.Users.Delete(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("用户 {Username} (ID: {UserId}) 已被删除", user.Username, user.Id);
    }
}