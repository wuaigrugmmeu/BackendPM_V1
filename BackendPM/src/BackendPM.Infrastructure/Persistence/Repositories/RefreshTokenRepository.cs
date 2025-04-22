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
    /// 根据令牌值查找刷新令牌
    /// </summary>
    public async Task<RefreshToken?> FindByTokenAsync(string token)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    /// <summary>
    /// 根据令牌值获取刷新令牌（与FindByTokenAsync功能相同）
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
    /// 获取用户的所有有效刷新令牌
    /// </summary>
    public async Task<List<RefreshToken>> GetUserActiveTokensAsync(Guid userId)
    {
        return await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsUsed && !rt.IsRevoked && rt.ExpiryTime > DateTime.UtcNow)
            .ToListAsync();
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
    /// 使令牌失效
    /// </summary>
    public async Task RevokeTokenAsync(string token)
    {
        var refreshToken = await FindByTokenAsync(token);
        if (refreshToken != null)
        {
            refreshToken.Revoke();
            _dbContext.RefreshTokens.Update(refreshToken);
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 使用户的所有令牌失效
    /// </summary>
    public async Task RevokeAllUserTokensAsync(Guid userId)
    {
        var tokens = await GetAllByUserIdAsync(userId);
        foreach (var token in tokens)
        {
            token.Revoke();
            _dbContext.RefreshTokens.Update(token);
        }
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 清理过期的刷新令牌
    /// </summary>
    public async Task CleanupExpiredTokensAsync()
    {
        // 查找所有过期、已使用或已撤销的令牌
        var expiredTokens = await _dbContext.RefreshTokens
            .Where(rt => rt.IsUsed || rt.IsRevoked || rt.ExpiryTime <= DateTime.UtcNow)
            .ToListAsync();
        
        if (expiredTokens.Any())
        {
            _dbContext.RefreshTokens.RemoveRange(expiredTokens);
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 清理过期的刷新令牌（带返回值）
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

    public Task<RefreshToken?> GetByTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task<List<RefreshToken>> GetAllByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}