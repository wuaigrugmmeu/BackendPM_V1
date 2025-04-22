using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces.Repositories;
using BackendPM.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace BackendPM.Infrastructure.Persistence.Repositories;

/// <summary>
/// 刷新令牌仓储实现
/// </summary>
public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    /// <summary>
    /// 根据令牌值获取刷新令牌
    /// </summary>
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    /// <summary>
    /// 获取用户的所有刷新令牌
    /// </summary>
    public async Task<List<RefreshToken>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 获取用户的有效刷新令牌数量
    /// </summary>
    public async Task<int> CountActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .CountAsync(rt => rt.UserId == userId && !rt.IsUsed && !rt.IsRevoked && rt.ExpiryTime > DateTime.UtcNow, cancellationToken);
    }

    /// <summary>
    /// 清理过期的刷新令牌
    /// </summary>
    public async Task<int> CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        // 查找所有过期、已使用或已撤销的令牌
        var expiredTokens = await _dbContext.RefreshTokens
            .Where(rt => rt.IsUsed || rt.IsRevoked || rt.ExpiryTime <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        
        if (expiredTokens.Any())
        {
            _dbContext.RefreshTokens.RemoveRange(expiredTokens);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        
        return expiredTokens.Count;
    }

    public Task<RefreshToken?> FindByTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task<List<RefreshToken>> GetUserActiveTokensAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task RevokeTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task RevokeAllUserTokensAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task CleanupExpiredTokensAsync()
    {
        throw new NotImplementedException();
    }
}