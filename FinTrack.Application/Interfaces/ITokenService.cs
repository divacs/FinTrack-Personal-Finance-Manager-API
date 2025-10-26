using FinTrack.Domain.Entities;

namespace TaskFlow.Utility.Service
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(ApplicationUser user);
    }
}
