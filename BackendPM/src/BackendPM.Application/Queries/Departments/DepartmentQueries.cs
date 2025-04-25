using System;
using System.Collections.Generic;
using BackendPM.Application.DTOs;
using MediatR;

namespace BackendPM.Application.Queries.Departments;

/// <summary>
/// 获取所有部门查询
/// </summary>
public record GetAllDepartmentsQuery() : IRequest<List<DepartmentDto>>;

/// <summary>
/// 获取部门树形结构查询
/// </summary>
public record GetDepartmentTreeQuery() : IRequest<List<DepartmentTreeDto>>;

/// <summary>
/// 根据ID获取部门查询
/// </summary>
public record GetDepartmentByIdQuery(Guid Id) : IRequest<DepartmentDto>;

/// <summary>
/// 获取部门用户查询
/// </summary>
public record GetDepartmentUsersQuery(Guid DepartmentId) : IRequest<List<UserDto>>;

/// <summary>
/// 获取用户所属部门查询
/// </summary>
public record GetUserDepartmentsQuery(Guid UserId) : IRequest<List<UserDepartmentDto>>;