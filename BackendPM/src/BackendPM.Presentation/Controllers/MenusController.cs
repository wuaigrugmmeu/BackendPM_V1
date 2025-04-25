using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackendPM.Application.Commands.Menus;
using BackendPM.Application.DTOs;
using BackendPM.Application.Queries.Menus;
using BackendPM.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendPM.Presentation.Controllers;

/// <summary>
/// 菜单管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MenusController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 构造函数
    /// </summary>
    public MenusController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// 获取所有菜单
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permissions.menus.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MenuDto>>> GetMenus()
    {
        var query = new GetAllMenusQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 获取菜单树形结构
    /// </summary>
    [HttpGet("tree")]
    [Authorize(Policy = "permissions.menus.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MenuTreeDto>>> GetMenuTree()
    {
        var query = new GetMenuTreeQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 根据ID获取菜单
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permissions.menus.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MenuDto>> GetMenu(Guid id)
    {
        try
        {
            var query = new GetMenuByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "permissions.menus.create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MenuDto>> CreateMenu([FromBody] CreateMenuRequest request)
    {
        try
        {
            var command = new CreateMenuCommand(
                request.Name,
                request.Code,
                request.Path,
                request.Icon,
                request.Component,
                request.ParentMenuId,
                request.SortOrder,
                request.Visible);

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetMenu), new { id = result.Id }, result);
        }
        catch (BusinessRuleViolationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "permissions.menus.edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MenuDto>> UpdateMenu(Guid id, [FromBody] UpdateMenuRequest request)
    {
        try
        {
            var command = new UpdateMenuCommand(
                id,
                request.Name,
                request.Path,
                request.Icon,
                request.Component,
                request.ParentMenuId,
                request.SortOrder,
                request.Visible);

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
    /// 删除菜单
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "permissions.menus.delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteMenu(Guid id)
    {
        try
        {
            var command = new DeleteMenuCommand(id);
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
    /// 获取角色菜单
    /// </summary>
    [HttpGet("roles/{roleId}")]
    [Authorize(Policy = "permissions.menus.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<MenuDto>>> GetRoleMenus(Guid roleId)
    {
        try
        {
            var query = new GetRoleMenusQuery(roleId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// 获取用户菜单树形结构
    /// </summary>
    [HttpGet("users/{userId}/tree")]
    [Authorize(Policy = "permissions.menus.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<MenuTreeDto>>> GetUserMenuTree(Guid userId)
    {
        try
        {
            var query = new GetUserMenuTreeQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// 添加菜单到角色
    /// </summary>
    [HttpPost("{menuId}/roles/{roleId}")]
    [Authorize(Policy = "permissions.menus.edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddMenuToRole(Guid menuId, Guid roleId)
    {
        try
        {
            var command = new AssignMenuToRoleCommand(menuId, roleId);
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
    /// 从角色移除菜单
    /// </summary>
    [HttpDelete("{menuId}/roles/{roleId}")]
    [Authorize(Policy = "permissions.menus.edit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RemoveMenuFromRole(Guid menuId, Guid roleId)
    {
        try
        {
            var command = new RemoveMenuFromRoleCommand(menuId, roleId);
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
}

/// <summary>
/// 创建菜单请求
/// </summary>
public class CreateMenuRequest
{
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 路由路径
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 组件
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 父菜单ID
    /// </summary>
    public Guid? ParentMenuId { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool Visible { get; set; } = true;
}

/// <summary>
/// 更新菜单请求
/// </summary>
public class UpdateMenuRequest
{
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 路由路径
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 组件
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 父菜单ID
    /// </summary>
    public Guid? ParentMenuId { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool Visible { get; set; } = true;
}