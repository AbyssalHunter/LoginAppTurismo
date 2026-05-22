using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace appTurismo.Services
{
    public interface IUserService
    {
        Task<string?> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string email, string password, Models.Supabase.User profileData);
        Task<bool> RegisterWithRoleAsync(string email, string password, Models.Supabase.User profileData, string roleName);
        Task LogoutAsync();
        bool IsUserLoggedIn();
        Task DeleteUserAsync(Guid id);
        Task<List<Models.UserDTO>> GetUsersAsync();
        Task CreateUserAsync(Models.Supabase.User user);
        Task UpdateUserAsync(Models.Supabase.User user);
    }
}