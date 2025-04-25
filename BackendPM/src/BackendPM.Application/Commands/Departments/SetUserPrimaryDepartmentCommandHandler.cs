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
/// 设置用户主部门命令处理器
/// </summary>
public class SetUserPrimaryDepartmentCommandHandler : IRequestHandler<SetUserPrimaryDepartmentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SetUserPrimaryDepartmentCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public SetUserPrimaryDepartmentCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<SetUserPrimaryDepartmentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理设置用户主部门命令
    /// </summary>
    public async Task Handle(SetUserPrimaryDepartmentCommand request, CancellationToken cancellationToken)
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

        // 如果已经是主部门，则无需操作
        if (userDepartment.IsPrimary)
        {
            return;
        }

        // 将用户当前的主部门（如果有）设置为非主部门
        var currentPrimaryDepartments = await _unitOfWork.Users
            .AsQueryable()
            .Where(u => u.Id == request.UserId)
            .SelectMany(u => u.UserDepartments)
            .Where(ud => ud.IsPrimary)
            .ToListAsync(cancellationToken);

        foreach (var primaryDepartment in currentPrimaryDepartments)
        {
            primaryDepartment.SetAsPrimary(false);
        }

        // 设置新的主部门
        userDepartment.SetAsPrimary(true);
        
        // 保存更改
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "已将部门 {DepartmentName} (ID: {DepartmentId}) 设置为用户 {Username} (ID: {UserId}) 的主部门", 
            department.Name, department.Id, user.Username, user.Id);
    }
}