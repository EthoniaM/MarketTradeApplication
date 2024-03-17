
using System.ComponentModel.DataAnnotations;

namespace MarketTradeApplication.ViewM
{
    public class RegisterVM
    {
        [Required]
        public string FirstName { get; set; } = default!;
        [Required]
        public string LastName { get; set; } = default!;
        [Required(ErrorMessage = "Enter email")]
        public string Email { get; set; } = default!;
        [Required(ErrorMessage = "Enter password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and alpha numeric", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;


    }
}
