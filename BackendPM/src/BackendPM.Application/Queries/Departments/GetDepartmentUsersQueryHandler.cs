using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Entities;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendPM.Application.Queries.Departments;

/// <summary>
/// 获取部门用户查询处理器
/// </summary>
public class GetDepartmentUsersQueryHandler : IRequestHandler<GetDepartmentUsersQuery, List<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    public GetDepartmentUsersQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// 处理获取部门用户查询
    /// </summary>
    public async Task<List<UserDto>> Handle(GetDepartmentUsersQuery request, CancellationToken cancellationToken)
    {
        // 首先检查部门是否存在
        var department = await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId);
        if (department == null)
        {
            throw new EntityNotFoundException($"找不到ID为 {request.DepartmentId} 的部门");
        }

        // 获取该部门下的所有用户
        var users = await _unitOfWork.Departments
            .AsQueryable()
            .Where(d => d.Id == request.DepartmentId)
            .SelectMany(d => d.UserDepartments)
            .Include(ud => ud.User)
            .Select(ud => ud.User)
            .OrderBy(u => u.Username)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<UserDto>>(users);
    }
}