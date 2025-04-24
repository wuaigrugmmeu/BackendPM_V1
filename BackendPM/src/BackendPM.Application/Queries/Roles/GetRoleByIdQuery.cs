using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Constants;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;

namespace BackendPM.Application.Queries.Roles;

/// <summary>
/// 根据ID获取角色查询
/// </summary>
public class GetRoleByIdQuery : BaseQuery<RoleDto>
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; }
    
    public GetRoleByIdQuery(Guid roleId)
    {
        RoleId = roleId;
    }
}

/// <summary>
/// 根据ID获取角色查询处理器
/// </summary>
public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
{
    private readonly IUnitOfWork _unitOfWork;
    
    public GetRoleByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<RoleDto> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken)
    {
        var role = await _unitOfWork.Roles.GetByIdWithPermissionsAsync(query.RoleId)
            ?? throw new EntityNotFoundException(ErrorMessages.EntityNames.RoleType, query.RoleId);
            
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Code = role.Code,
            Description = role.Description,
            IsSystemRole = role.IsSystem,
            CreatedAt = role.CreatedAt,
            LastModifiedAt = role.LastModifiedAt,
            Permissions = role.RolePermissions
                .Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                    Code = rp.Permission.Code,
                    Group = rp.Permission.Group,
                    Description = rp.Permission.Description
                })
                .ToList()
        };
    }
}