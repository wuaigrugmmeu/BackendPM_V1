using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendPM.Application.Queries.Menus;

/// <summary>
/// 根据ID获取菜单查询处理器
/// </summary>
public class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQuery, MenuDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    public GetMenuByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// 处理根据ID获取菜单查询
    /// </summary>
    public async Task<MenuDto> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
    {
        var menu = await _unitOfWork.Menus
            .AsQueryable()
            .Include(m => m.ParentMenu)
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (menu == null)
        {
            throw new EntityNotFoundException($"找不到ID为 {request.Id} 的菜单");
        }

        return _mapper.Map<MenuDto>(menu);
    }
}