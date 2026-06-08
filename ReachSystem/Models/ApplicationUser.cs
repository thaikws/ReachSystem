using Microsoft.AspNetCore.Identity;

namespace ReachSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; }
    }
}
