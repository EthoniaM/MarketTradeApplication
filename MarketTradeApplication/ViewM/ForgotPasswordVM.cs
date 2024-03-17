
using System.ComponentModel.DataAnnotations;

namespace MarketTradeApplication.ViewM
{
    public class ForgotPasswordVM
    {
        [Required]
        public string Email { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string Token { get; set; } = default!;
        [Required(ErrorMessage ="Please enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage ="Password and confirm password not matched")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
