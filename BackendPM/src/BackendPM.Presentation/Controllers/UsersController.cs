using BackendPM.Application.DTOs;
using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace BackendPM.Presentation.Controllers;

/// <summary>
/// 用户管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUnitOfWork unitOfWork, ILogger<UsersController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有用户
    /// </summary>
    /// <param name="queryParams">查询参数</param>
    /// <returns>用户列表</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserDto>>> GetUsers([FromQuery] UserQueryParams queryParams)
    {
        try
        {
            var (users, totalCount) = await _unitOfWork.Users.GetPagedAsync(
                queryParams.PageIndex,
                queryParams.PageSize,
                queryParams.SearchTerm);

            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                LastModifiedAt = u.LastModifiedAt,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            }).ToList();

            var result = new PagedResult<UserDto>
            {
                PageIndex = queryParams.PageIndex,
                PageSize = queryParams.PageSize,
                TotalCount = totalCount,
                Items = userDtos
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户列表时发生错误");
            return StatusCode(500, "内部服务器错误");
        }
    }

    /// <summary>
    /// 获取指定ID的用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户信息</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(id);
            if (user == null)
            {
                return NotFound($"用户ID为 {id} 的用户不存在");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastModifiedAt = user.LastModifiedAt,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户 {UserId} 信息时发生错误", id);
            return StatusCode(500, "内部服务器错误");
        }
    }

    /// <summary>
    /// 创建新用户
    /// </summary>
    /// <param name="createUserDto">创建用户请求</param>
    /// <returns>新创建的用户信息</returns>
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
    {
        try
        {
            // 检查用户名是否已存在
            if (await _unitOfWork.Users.ExistsAsync(u => u.Username == createUserDto.Username))
            {
                return BadRequest($"用户名 '{createUserDto.Username}' 已被使用");
            }

            // 检查电子邮件是否已存在
            if (await _unitOfWork.Users.ExistsAsync(u => u.Email == createUserDto.Email))
            {
                return BadRequest($"电子邮件 '{createUserDto.Email}' 已被使用");
            }

            // 对密码进行哈希处理
            string passwordHash = HashPassword(createUserDto.Password);

            // 创建新用户（通过构造函数传入必要参数，并在构造后调用UpdateProfile设置全名）
            var user = new User(createUserDto.Username, createUserDto.Email, passwordHash);
            
            // 如果有全名，则通过UpdateProfile方法设置
            if (!string.IsNullOrEmpty(createUserDto.FullName))
            {
                user.UpdateProfile(createUserDto.Email, createUserDto.FullName);
            }

            // 添加角色
            if (createUserDto.RoleIds?.Any() == true)
            {
                foreach (var roleId in createUserDto.RoleIds)
                {
                    var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
                    if (role != null)
                    {
                        user.AddRole(role);
                    }
                }
            }

            // 保存用户
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // 返回创建的用户
            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastModifiedAt = user.LastModifiedAt,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建用户时发生错误");
            return StatusCode(500, "内部服务器错误");
        }
    }

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="updateUserDto">更新用户请求</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound($"用户ID为 {id} 的用户不存在");
            }

            // 检查邮箱是否被其他用户使用
            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                if (await _unitOfWork.Users.ExistsAsync(u => u.Email == updateUserDto.Email))
                {
                    return BadRequest($"电子邮件 '{updateUserDto.Email}' 已被使用");
                }

                // 更新邮箱
                user.UpdateProfile(updateUserDto.Email, updateUserDto.FullName);
            }
            else if (updateUserDto.FullName != user.FullName)
            {
                // 只更新全名
                user.UpdateProfile(user.Email, updateUserDto.FullName);
            }

            // 更新用户状态
            if (updateUserDto.IsActive.HasValue && updateUserDto.IsActive.Value != user.IsActive)
            {
                user.SetActiveStatus(updateUserDto.IsActive.Value);
            }

            // 保存更改
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用户 {UserId} 时发生错误", id);
            return StatusCode(500, "内部服务器错误");
        }
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound($"用户ID为 {id} 的用户不存在");
            }

            // 删除用户
            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除用户 {UserId} 时发生错误", id);
            return StatusCode(500, "内部服务器错误");
        }
    }

    /// <summary>
    /// 简单的密码哈希实现（在生产环境应使用更安全的方法，如BCrypt）
    /// </summary>
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}