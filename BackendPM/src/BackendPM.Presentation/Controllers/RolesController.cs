using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackendPM.Application.Commands.Roles;
using BackendPM.Application.DTOs;
using BackendPM.Application.Queries.Roles;
using BackendPM.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendPM.Presentation.Controllers;

/// <summary>
/// 角色控制器 - 使用中介者模式完全解耦
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RolesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// 获取所有角色
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "roles.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RoleDto>>> GetRoles()
    {
        var query = new GetAllRolesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "roles.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleDto>> GetRole(Guid id)
    {
        try
        {
            var query = new GetRoleByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "roles.create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleRequest request)
    {
        try
        {
            var command = new CreateRoleCommand(
                request.Name,
                request.Code,
                request.Description,
                request.PermissionIds);

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetRole), new { id = result.Id }, result);
        }
        catch (Exception ex) when (ex.Message.Contains("已存在"))
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "roles.edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleDto>> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request)
    {
        try
        {
            var command = new UpdateRoleCommand(
                id,
                request.Name,
                request.Description,
                request.PermissionIds);

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
    /// 删除角色
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "roles.delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteRole(Guid id)
    {
        try
        {
            var command = new DeleteRoleCommand(id);
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
}

/// <summary>
/// 创建角色请求
/// </summary>
public class CreateRoleRequest
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 权限ID列表
    /// </summary>
    public List<Guid> PermissionIds { get; set; } = [];
}

/// <summary>
/// 更新角色请求
/// </summary>
public class UpdateRoleRequest
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 权限ID列表
    /// </summary>
    public List<Guid> PermissionIds { get; set; } = [];
}