using System;
using BackendPM.Application.DTOs;
using MediatR;

namespace BackendPM.Application.Commands.Departments;

/// <summary>
/// 创建部门命令
/// </summary>
public record CreateDepartmentCommand(
    string Name,
    string Code,
    string? Description,
    Guid? ParentDepartmentId,
    int SortOrder) : IRequest<DepartmentDto>;

/// <summary>
/// 更新部门命令
/// </summary>
public record UpdateDepartmentCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid? ParentDepartmentId,
    int SortOrder) : IRequest<DepartmentDto>;

/// <summary>
/// 删除部门命令
/// </summary>
public record DeleteDepartmentCommand(Guid Id) : IRequest;

/// <summary>
/// 添加用户到部门命令
/// </summary>
public record AddUserToDepartmentCommand(
    Guid UserId,
    Guid DepartmentId,
    bool IsPrimary = false) : IRequest;

/// <summary>
/// 从部门移除用户命令
/// </summary>
public record RemoveUserFromDepartmentCommand(
    Guid UserId,
    Guid DepartmentId) : IRequest;

/// <summary>
/// 设置用户主部门命令
/// </summary>
public record SetUserPrimaryDepartmentCommand(
    Guid UserId,
    Guid DepartmentId) : IRequest;