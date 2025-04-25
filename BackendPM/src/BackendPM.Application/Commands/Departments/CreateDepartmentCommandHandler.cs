using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Entities;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Commands.Departments;

/// <summary>
/// 创建部门命令处理器
/// </summary>
public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, DepartmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateDepartmentCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public CreateDepartmentCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateDepartmentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理创建部门命令
    /// </summary>
    public async Task<DepartmentDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        // 检查编码是否已存在
        var existingDepartment = await _unitOfWork.Departments.GetByCodeAsync(request.Code);
        if (existingDepartment != null)
        {
            throw new BusinessRuleViolationException($"编码为 {request.Code} 的部门已存在");
        }

        // 检查父部门是否存在
        Department? parentDepartment = null;
        if (request.ParentDepartmentId.HasValue)
        {
            parentDepartment = await _unitOfWork.Departments.GetByIdAsync(request.ParentDepartmentId.Value);
            if (parentDepartment == null)
            {
                throw new EntityNotFoundException($"找不到ID为 {request.ParentDepartmentId.Value} 的父部门");
            }
        }

        // 创建部门实体
        var department = new Department(
            Guid.NewGuid(),
            request.Name,
            request.Code,
            request.Description,
            request.ParentDepartmentId,
            request.SortOrder,
            isSystem: false);

        // 保存到数据库
        await _unitOfWork.Departments.AddAsync(department);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("已创建部门 {DepartmentName} (ID: {DepartmentId})", department.Name, department.Id);

        // 返回DTO
        return _mapper.Map<DepartmentDto>(department);
    }
}