using System;
using System.Threading;
using System.Threading.Tasks;
using BackendPM.Domain.Entities;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Commands.Departments;

/// <summary>
/// 添加用户到部门命令处理器
/// </summary>
public class AddUserToDepartmentCommandHandler : IRequestHandler<AddUserToDepartmentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddUserToDepartmentCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AddUserToDepartmentCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<AddUserToDepartmentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理添加用户到部门命令
    /// </summary>
    public async Task Handle(AddUserToDepartmentCommand request, CancellationToken cancellationToken)
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

        // 检查用户是否已经在该部门中
        var isUserAlreadyInDepartment = await _unitOfWork.Users
            .AsQueryable()
            .Where(u => u.Id == request.UserId)
            .SelectMany(u => u.UserDepartments)
            .AnyAsync(ud => ud.DepartmentId == request.DepartmentId, cancellationToken);

        if (isUserAlreadyInDepartment)
        {
            throw new BusinessRuleViolationException($"用户 '{user.Username}' 已经在部门 '{department.Name}' 中");
        }

        // 如果设置为主部门，需要将用户当前的主部门（如果有）设置为非主部门
        if (request.IsPrimary)
        {
            var currentPrimaryDepartments = await _unitOfWork.Users
                .AsQueryable()
                .Where(u => u.Id == request.UserId)
                .SelectMany(u => u.UserDepartments)
                .Where(ud => ud.IsPrimary)
                .ToListAsync(cancellationToken);

            foreach (var userDepartment in currentPrimaryDepartments)
            {
                userDepartment.SetAsPrimary(false);
            }
        }

        // 创建用户部门关联
        var userDepartment = new UserDepartment(
            request.UserId,
            request.DepartmentId,
            request.IsPrimary);

        // 将用户添加到部门
        await user.AddToDepartmentAsync(department, userDepartment);
        
        // 保存更改
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "已将用户 {Username} (ID: {UserId}) 添加到部门 {DepartmentName} (ID: {DepartmentId}), IsPrimary: {IsPrimary}", 
            user.Username, user.Id, department.Name, department.Id, request.IsPrimary);
    }
}