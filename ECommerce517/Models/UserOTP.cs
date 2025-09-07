using System.ComponentModel.DataAnnotations;

namespace ECommerce517.Models
{
    public class UserOTP
    {
        [Key]
        public int Id { get; set; }
        
        public string? ApplicationUserId { get; set; }
        [Required]
        public ApplicationUser? applicationUser { get; set; }

        public  int OTPNumber { get; set; }

        public DateTime ValidTo { get; set; }
        
    }
}
