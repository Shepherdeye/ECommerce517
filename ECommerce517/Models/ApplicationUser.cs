using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

namespace ECommerce517.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Name { get; set; }=string.Empty;
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string?  ProfileImage { get; set; }
    }
}
