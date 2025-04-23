using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;

namespace BackendPM.Application.Queries.Users;

/// <summary>
/// 获取用户列表查询
/// </summary>
public class GetUsersQuery : IRequest<PagedResult<UserDto>>
{
    /// <summary>
    /// 页码，从1开始
    /// </summary>
    public int PageIndex { get; }
    
    /// <summary>
    /// 每页记录数
    /// </summary>
    public int PageSize { get; }
    
    /// <summary>
    /// 搜索关键词
    /// </summary>
    public string? SearchTerm { get; }

    public GetUsersQuery(int pageIndex, int pageSize, string? searchTerm = null)
    {
        PageIndex = pageIndex <= 0 ? 1 : pageIndex;
        PageSize = pageSize <= 0 ? 10 : pageSize;
        SearchTerm = searchTerm;
    }
}

/// <summary>
/// 获取用户列表查询处理器
/// </summary>
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var (users, totalCount) = await _unitOfWork.Users.GetPagedListAsync(
            query.PageIndex,
            query.PageSize,
            query.SearchTerm);

        var userDtos = _mapper.Map<List<UserDto>>(users);

        return new PagedResult<UserDto>
        {
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            Items = userDtos
        };
    }
}