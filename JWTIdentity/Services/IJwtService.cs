using JWTIdentity.Entities;

namespace JWTIdentity.Services
{
    public interface IJwtService
    {
        Task<string> CreateTokenAsync(AppUser user);
    }
}
