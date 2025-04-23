using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;

namespace BackendPM.Application.Queries.Roles;

/// <summary>
/// 获取所有角色查询
/// </summary>
public class GetAllRolesQuery : BaseQuery<List<RoleDto>>
{
    // 无需额外参数
}

/// <summary>
/// 获取所有角色查询处理器
/// </summary>
public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    
    public GetAllRolesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<List<RoleDto>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken = default)
    {
        var roles = await _unitOfWork.Roles.GetAllWithPermissionsAsync();
        
        return roles.Select(role => new RoleDto
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
        }).ToList();
    }
}