using FinTrack.Domain.Entities;

namespace FinTrack.Application.Interfaces
{
    // Interface for managing and retrieving user data.
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task UpdateUserAsync(ApplicationUser user);
    }
}
