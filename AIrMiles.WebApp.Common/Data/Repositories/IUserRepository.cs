using AIrMiles.WebApp.Common.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Only Works for BackOffice users!
        /// </summary>
        /// <param name="user">BackOffice User</param>
        /// <returns></returns>
        Task<string> GetUserMainRoleAsync(User user);

        ICollection<User> GetIndexList();

        IEnumerable<SelectListItem> GetBackOfficeRoles();

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task AddUsertoRoleAsync(User user, string roleName);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task CheckRoleAsync(string roleName);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(string userId);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<SignInResult> LoginAsync(string username, string password, bool rememberMe);

        Task LogoutAsync();

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        Task<IdentityResult> UpdateUserAsync(User user);
        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        Task<IdentityResult> RemoveFromRole(User user, string role);
    }
}
