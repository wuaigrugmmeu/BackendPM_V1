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
public class GetUserByIdQuery : BaseQuery<UserDto>
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; }
    
    public GetUserByIdQuery(Guid userId)
    {
        UserId = userId;
    }
}

/// <summary>
/// 根据ID获取用户查询处理器
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    
    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<UserDto> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default)
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