using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;

namespace BackendPM.Application.Queries.Departments;

/// <summary>
/// 获取部门树形结构查询处理器
/// </summary>
public class GetDepartmentTreeQueryHandler : IRequestHandler<GetDepartmentTreeQuery, List<DepartmentTreeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    public GetDepartmentTreeQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// 处理获取部门树形结构查询
    /// </summary>
    public async Task<List<DepartmentTreeDto>> Handle(GetDepartmentTreeQuery request, CancellationToken cancellationToken)
    {
        // 获取所有部门并构建树形结构
        var departments = await _unitOfWork.Departments.GetAllWithHierarchyAsync();
        
        // 获取根部门（没有父部门的部门）
        var rootDepartments = departments
            .Where(d => d.ParentDepartmentId == null)
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.Name)
            .ToList();

        // 构建树形结构
        var result = new List<DepartmentTreeDto>();
        foreach (var department in rootDepartments)
        {
            result.Add(BuildDepartmentTree(department, departments));
        }

        return result;
    }

    private DepartmentTreeDto BuildDepartmentTree(Department department, List<Department> allDepartments)
    {
        var dto = _mapper.Map<DepartmentTreeDto>(department);
        
        // 获取当前部门的子部门
        var children = allDepartments
            .Where(d => d.ParentDepartmentId == department.Id)
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.Name)
            .ToList();

        // 递归构建子部门的树形结构
        foreach (var child in children)
        {
            dto.Children.Add(BuildDepartmentTree(child, allDepartments));
        }

        return dto;
    }
}