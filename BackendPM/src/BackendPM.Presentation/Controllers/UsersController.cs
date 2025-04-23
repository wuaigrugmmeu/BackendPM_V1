using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackendPM.Application.Commands.Users;
using BackendPM.Application.DTOs;
using BackendPM.Application.Queries.Users;
using BackendPM.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendPM.Presentation.Controllers;

/// <summary>
/// 用户控制器 - 使用中介者模式完全解耦
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 获取所有用户
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "users.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var query = new GetAllUsersQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "users.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        try
        {
            var query = new GetUserByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "users.create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var command = new CreateUserCommand(
                request.Username,
                request.Email,
                request.Password,
                request.FullName);

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
        }
        catch (Exception ex) when (ex.Message.Contains("已被使用"))
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "users.edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var command = new UpdateUserCommand(
                id,
                request.Email,
                request.FullName,
                request.IsActive);

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex) when (ex.Message.Contains("已被使用"))
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "users.delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        try
        {
            var command = new DeleteUserCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }
}

/// <summary>
/// 创建用户请求
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// 电子邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// 全名
    /// </summary>
    public string? FullName { get; set; }
}

/// <summary>
/// 更新用户请求
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// 电子邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// 全名
    /// </summary>
    public string? FullName { get; set; }
    
    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; }
}