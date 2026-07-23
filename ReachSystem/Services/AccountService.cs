using Microsoft.AspNetCore.Identity;
using ReachSystem.Models;

namespace ReachSystem.Services
{
    public class AccountService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountService(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<SignInResult> LoginAsync(string email, string senha, bool rememberMe)
        {
            return await _signInManager.PasswordSignInAsync(
                email,
                senha,
                isPersistent: rememberMe,
                lockoutOnFailure: false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
