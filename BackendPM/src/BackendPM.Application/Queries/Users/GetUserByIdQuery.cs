using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Constants;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;

namespace BackendPM.Application.Queries.Users;

/// <summary>
/// 根据ID获取用户查询
/// </summary>
public class GetUserByIdQuery(Guid userId) : BaseQuery<UserDto>
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; } = userId;
}

/// <summary>
/// 根据ID获取用户查询处理器
/// </summary>
public class GetUserByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<UserDto> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdWithRolesAsync(query.UserId)
            ?? throw new EntityNotFoundException(ErrorMessages.EntityNames.UserType, query.UserId);

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

        return userDto;
    }
}