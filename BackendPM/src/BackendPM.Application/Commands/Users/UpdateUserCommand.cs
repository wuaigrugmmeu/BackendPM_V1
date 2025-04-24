using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Constants;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Commands.Users;

/// <summary>
/// 更新用户命令
/// </summary>
public class UpdateUserCommand : BaseCommand<UserDto>
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; }
    
    /// <summary>
    /// 电子邮箱
    /// </summary>
    public string Email { get; }
    
    /// <summary>
    /// 全名
    /// </summary>
    public string? FullName { get; }
    
    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; }
    
    public UpdateUserCommand(Guid userId, string email, string? fullName, bool isActive)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        FullName = fullName;
        IsActive = isActive;
    }
}

/// <summary>
/// 更新用户命令处理器
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    
    public UpdateUserCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<UserDto> Handle(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        // 获取用户
        var user = await _unitOfWork.Users.GetByIdWithRolesAsync(command.UserId)
            ?? throw new EntityNotFoundException(ErrorMessages.EntityNames.UserType, command.UserId);
        
        // 检查电子邮件是否被其他用户使用
        if (user.Email != command.Email && 
            await _unitOfWork.Users.ExistsAsync(u => u.Email == command.Email && u.Id != command.UserId))
        {
            throw new InvalidOperationException(string.Format(ErrorMessages.User.EmailAlreadyUsed, command.Email));
        }
        
        // 更新用户信息
        user.UpdateProfile(command.Email, command.FullName);
        
        // 更新用户状态
        if (user.IsActive != command.IsActive)
        {
            user.SetActiveStatus(command.IsActive);
        }
        
        // 保存更改
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("用户 {Username} (ID: {UserId}) 的信息已更新", user.Username, user.Id);
        
        // 返回更新后的用户DTO
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastModifiedAt = user.LastModifiedAt,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }
}