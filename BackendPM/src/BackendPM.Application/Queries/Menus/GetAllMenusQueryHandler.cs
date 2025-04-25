using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendPM.Application.Queries.Menus;

/// <summary>
/// 获取所有菜单查询处理器
/// </summary>
public class GetAllMenusQueryHandler : IRequestHandler<GetAllMenusQuery, List<MenuDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    public GetAllMenusQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// 处理获取所有菜单查询
    /// </summary>
    public async Task<List<MenuDto>> Handle(GetAllMenusQuery request, CancellationToken cancellationToken)
    {
        var menus = await _unitOfWork.Menus
            .AsQueryable()
            .Include(m => m.ParentMenu)
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.Name)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<MenuDto>>(menus);
    }
}