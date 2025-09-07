using System.ComponentModel.DataAnnotations;

namespace ECommerce517.ViewModels
{
    public class LoginVM
    {
        public int Id { get; set; }
        [Required]
        public string UserNameOrEmail { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;       
        public bool RememberMe { get; set; }

    }
}
