using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Commands.Roles;

/// <summary>
/// 删除角色命令
/// </summary>
public class DeleteRoleCommand : BaseCommand
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; }
    
    public DeleteRoleCommand(Guid roleId)
    {
        RoleId = roleId;
    }
}

/// <summary>
/// 删除角色命令处理器
/// </summary>
public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteRoleCommandHandler> _logger;
    
    public DeleteRoleCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteRoleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task Handle(DeleteRoleCommand command, CancellationToken cancellationToken = default)
    {
        // 获取角色实体
        var role = await _unitOfWork.Roles.GetByIdAsync(command.RoleId)
            ?? throw new EntityNotFoundException("Role", command.RoleId);
            
        // 检查是否为系统角色
        if (role.IsSystem)
        {
            throw new BusinessRuleViolationException("系统角色不允许删除");
        }
        
        // 检查角色是否有关联的用户
        var usersWithRole = await _unitOfWork.Users.FindAsync(u => u.UserRoles.Any(ur => ur.RoleId == command.RoleId));
        if (usersWithRole.Any())
        {
            throw new BusinessRuleViolationException($"无法删除角色 '{role.Name}'，因为它已分配给{usersWithRole.Count}个用户");
        }
        
        // 删除角色
        _unitOfWork.Roles.Delete(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("角色 {RoleName} (ID: {RoleId}) 已被删除", role.Name, role.Id);
    }
}