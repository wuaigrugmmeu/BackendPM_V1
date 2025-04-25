using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Commands.Departments;

/// <summary>
/// 更新部门命令处理器
/// </summary>
public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, DepartmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateDepartmentCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public UpdateDepartmentCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateDepartmentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理更新部门命令
    /// </summary>
    public async Task<DepartmentDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        // 获取要更新的部门
        var department = await _unitOfWork.Departments.GetByIdAsync(request.Id);
        if (department == null)
        {
            throw new EntityNotFoundException($"找不到ID为 {request.Id} 的部门");
        }

        // 检查是否为系统预设部门
        if (department.IsSystem)
        {
            throw new BusinessRuleViolationException("不能修改系统预设部门");
        }

        // 检查父部门是否存在
        if (request.ParentDepartmentId.HasValue)
        {
            var parentDepartment = await _unitOfWork.Departments.GetByIdAsync(request.ParentDepartmentId.Value);
            if (parentDepartment == null)
            {
                throw new EntityNotFoundException($"找不到ID为 {request.ParentDepartmentId.Value} 的父部门");
            }

            // 确保不会创建循环引用
            if (request.ParentDepartmentId.Value == department.Id)
            {
                throw new BusinessRuleViolationException("部门不能将自己设为父部门");
            }
        }

        // 更新部门信息
        department.UpdateInfo(
            request.Name,
            request.Description,
            request.ParentDepartmentId,
            request.SortOrder);

        // 保存到数据库
        _unitOfWork.Departments.Update(department);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("已更新部门 {DepartmentName} (ID: {DepartmentId})", department.Name, department.Id);

        // 返回DTO
        return _mapper.Map<DepartmentDto>(department);
    }
}