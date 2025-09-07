using System.ComponentModel.DataAnnotations;

namespace ECommerce517.ViewModels
{
    public class UpdatePersonalInfoVM
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string? PhoneNumber { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? ProfileImage { get; set; }
        //[DataType(DataType.Password)]
        //public string? Password { get; set; }
        //[DataType(DataType.Password),Compare(nameof(Password))]
        //public string? ConfirmPassword { get; set; }
    }
}
