using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Commands.Departments;

/// <summary>
/// 从部门移除用户命令处理器
/// </summary>
public class RemoveUserFromDepartmentCommandHandler : IRequestHandler<RemoveUserFromDepartmentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveUserFromDepartmentCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public RemoveUserFromDepartmentCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<RemoveUserFromDepartmentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理从部门移除用户命令
    /// </summary>
    public async Task Handle(RemoveUserFromDepartmentCommand request, CancellationToken cancellationToken)
    {
        // 检查用户是否存在
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new EntityNotFoundException($"找不到ID为 {request.UserId} 的用户");
        }

        // 检查部门是否存在
        var department = await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId);
        if (department == null)
        {
            throw new EntityNotFoundException($"找不到ID为 {request.DepartmentId} 的部门");
        }

        // 获取用户部门关联
        var userDepartment = await _unitOfWork.Users
            .AsQueryable()
            .Where(u => u.Id == request.UserId)
            .SelectMany(u => u.UserDepartments)
            .FirstOrDefaultAsync(ud => ud.DepartmentId == request.DepartmentId, cancellationToken);

        if (userDepartment == null)
        {
            throw new EntityNotFoundException($"用户 '{user.Username}' 不在部门 '{department.Name}' 中");
        }

        // 如果是用户的主部门，则需要检查用户是否有其他部门，有则将第一个设为主部门
        if (userDepartment.IsPrimary)
        {
            var otherDepartments = await _unitOfWork.Users
                .AsQueryable()
                .Where(u => u.Id == request.UserId)
                .SelectMany(u => u.UserDepartments)
                .Where(ud => ud.DepartmentId != request.DepartmentId)
                .ToListAsync(cancellationToken);

            if (otherDepartments.Any())
            {
                otherDepartments.First().SetAsPrimary(true);
            }
        }

        // 从部门移除用户
        user.RemoveFromDepartment(department.Id);
        
        // 保存更改
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "已从部门 {DepartmentName} (ID: {DepartmentId}) 移除用户 {Username} (ID: {UserId})", 
            department.Name, department.Id, user.Username, user.Id);
    }
}