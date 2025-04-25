using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackendPM.Application.Commands.Departments;
using BackendPM.Application.DTOs;
using BackendPM.Application.Queries.Departments;
using BackendPM.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendPM.Presentation.Controllers;

/// <summary>
/// 部门管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 构造函数
    /// </summary>
    public DepartmentsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// 获取所有部门
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permissions.departments.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DepartmentDto>>> GetDepartments()
    {
        var query = new GetAllDepartmentsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 获取部门树形结构
    /// </summary>
    [HttpGet("tree")]
    [Authorize(Policy = "permissions.departments.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DepartmentTreeDto>>> GetDepartmentTree()
    {
        var query = new GetDepartmentTreeQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 根据ID获取部门
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permissions.departments.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DepartmentDto>> GetDepartment(Guid id)
    {
        try
        {
            var query = new GetDepartmentByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// 创建部门
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "permissions.departments.create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {
        try
        {
            var command = new CreateDepartmentCommand(
                request.Name,
                request.Code,
                request.Description,
                request.ParentDepartmentId,
                request.SortOrder);

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetDepartment), new { id = result.Id }, result);
        }
        catch (BusinessRuleViolationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 更新部门
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "permissions.departments.edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DepartmentDto>> UpdateDepartment(Guid id, [FromBody] UpdateDepartmentRequest request)
    {
        try
        {
            var command = new UpdateDepartmentCommand(
                id,
                request.Name,
                request.Description,
                request.ParentDepartmentId,
                request.SortOrder);

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleViolationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "permissions.departments.delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteDepartment(Guid id)
    {
        try
        {
            var command = new DeleteDepartmentCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleViolationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 获取部门用户列表
    /// </summary>
    [HttpGet("{id}/users")]
    [Authorize(Policy = "permissions.departments.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<UserDto>>> GetDepartmentUsers(Guid id)
    {
        try
        {
            var query = new GetDepartmentUsersQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// 添加用户到部门
    /// </summary>
    [HttpPost("{departmentId}/users/{userId}")]
    [Authorize(Policy = "permissions.departments.edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddUserToDepartment(Guid departmentId, Guid userId, [FromQuery] bool isPrimary = false)
    {
        try
        {
            var command = new AddUserToDepartmentCommand(userId, departmentId, isPrimary);
            await _mediator.Send(command);
            return Ok();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (BusinessRuleViolationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 从部门移除用户
    /// </summary>
    [HttpDelete("{departmentId}/users/{userId}")]
    [Authorize(Policy = "permissions.departments.edit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RemoveUserFromDepartment(Guid departmentId, Guid userId)
    {
        try
        {
            var command = new RemoveUserFromDepartmentCommand(userId, departmentId);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (BusinessRuleViolationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 设置用户的主部门
    /// </summary>
    [HttpPut("users/{userId}/primary/{departmentId}")]
    [Authorize(Policy = "permissions.departments.edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SetUserPrimaryDepartment(Guid userId, Guid departmentId)
    {
        try
        {
            var command = new SetUserPrimaryDepartmentCommand(userId, departmentId);
            await _mediator.Send(command);
            return Ok();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (BusinessRuleViolationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

/// <summary>
/// 创建部门请求
/// </summary>
public class CreateDepartmentRequest
{
    /// <summary>
    /// 部门名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 部门编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 部门描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 父部门ID
    /// </summary>
    public Guid? ParentDepartmentId { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }
}

/// <summary>
/// 更新部门请求
/// </summary>
public class UpdateDepartmentRequest
{
    /// <summary>
    /// 部门名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 部门描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 父部门ID
    /// </summary>
    public Guid? ParentDepartmentId { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }
}