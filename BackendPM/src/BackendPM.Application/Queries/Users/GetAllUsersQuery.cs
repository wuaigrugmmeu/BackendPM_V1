using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;

namespace BackendPM.Application.Queries.Users;

/// <summary>
/// 获取所有用户查询
/// </summary>
public class GetAllUsersQuery : BaseQuery<List<UserDto>>
{
    // 无需额外参数
}

/// <summary>
/// 获取所有用户查询处理器
/// </summary>
public class GetAllUsersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<List<UserDto>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllWithRolesAsync();

        return users.Select(user => new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastModifiedAt = user.LastModifiedAt,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        }).ToList();
    }
}