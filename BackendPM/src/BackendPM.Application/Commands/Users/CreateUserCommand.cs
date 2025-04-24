using BackendPM.Application.DTOs;
using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackendPM.Application.Commands.Users;

/// <summary>
/// 创建用户命令
/// </summary>
public class CreateUserCommand(string username, string email, string password, string? fullName = null, List<Guid>? roleIds = null) : BaseCommand<UserDto>
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; } = username ?? throw new ArgumentNullException(nameof(username));

    /// <summary>
    /// 电子邮箱
    /// </summary>
    public string Email { get; } = email ?? throw new ArgumentNullException(nameof(email));

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; } = password ?? throw new ArgumentNullException(nameof(password));

    /// <summary>
    /// 全名
    /// </summary>
    public string? FullName { get; } = fullName;

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<Guid>? RoleIds { get; } = roleIds;
}

/// <summary>
/// 创建用户命令处理器
/// </summary>
public class CreateUserCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateUserCommandHandler> logger) : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CreateUserCommandHandler> _logger = logger;

    public async Task<UserDto> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // 检查用户名是否已存在
        if (await _unitOfWork.Users.ExistsAsync(u => u.Username == command.Username))
        {
            throw new InvalidOperationException($"用户名 '{command.Username}' 已被使用");
        }

        // 检查电子邮件是否已存在
        if (await _unitOfWork.Users.ExistsAsync(u => u.Email == command.Email))
        {
            throw new InvalidOperationException($"电子邮件 '{command.Email}' 已被使用");
        }

        // 对密码进行哈希处理
        string passwordHash = HashPassword(command.Password);

        // 创建新用户
        var user = new User(command.Username, command.Email, passwordHash);

        // 如果有全名，则设置
        if (!string.IsNullOrEmpty(command.FullName))
        {
            user.UpdateProfile(command.Email, command.FullName);
        }

        // 添加角色
        if (command.RoleIds?.Any() == true)
        {
            foreach (var roleId in command.RoleIds)
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
                if (role != null)
                {
                    user.AddRole(role);
                }
            }
        }

        // 保存用户
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("创建了新用户 {Username} (ID: {UserId})", user.Username, user.Id);

        // 返回用户DTO
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

    /// <summary>
    /// 密码哈希处理
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}