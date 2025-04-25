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
/// 删除部门命令处理器
/// </summary>
public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteDepartmentCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public DeleteDepartmentCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteDepartmentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理删除部门命令
    /// </summary>
    public async Task Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        // 获取要删除的部门
        var department = await _unitOfWork.Departments.GetByIdAsync(request.Id);
        if (department == null)
        {
            throw new EntityNotFoundException($"找不到ID为 {request.Id} 的部门");
        }

        // 检查是否为系统预设部门
        if (department.IsSystem)
        {
            throw new BusinessRuleViolationException("不能删除系统预设部门");
        }

        // 检查是否有子部门
        var childDepartments = await _unitOfWork.Departments
            .AsQueryable()
            .Where(d => d.ParentDepartmentId == department.Id)
            .ToListAsync(cancellationToken);

        if (childDepartments.Any())
        {
            throw new BusinessRuleViolationException($"部门 '{department.Name}' 存在子部门，无法删除");
        }

        // 检查是否有关联用户
        var hasUsers = await _unitOfWork.Departments
            .AsQueryable()
            .Where(d => d.Id == department.Id)
            .SelectMany(d => d.UserDepartments)
            .AnyAsync(cancellationToken);

        if (hasUsers)
        {
            throw new BusinessRuleViolationException($"部门 '{department.Name}' 存在关联用户，无法删除");
        }

        // 删除部门
        _unitOfWork.Departments.Delete(department);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("已删除部门 {DepartmentName} (ID: {DepartmentId})", department.Name, department.Id);
    }
}