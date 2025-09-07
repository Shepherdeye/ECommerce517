using System.ComponentModel.DataAnnotations;

namespace ECommerce517.ViewModels
{
    public class RegisterVM
    {

        public int Id { get; set; } 
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; }= string.Empty;

        [Required,DataType(DataType.EmailAddress)]
        public string Email { get; set; }= string.Empty;

        [Required,DataType(DataType.Password)]
        public string Password { get; set; }= string.Empty;

        [Required, DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }= string.Empty;
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }

    }
}
