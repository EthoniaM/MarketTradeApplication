using System.ComponentModel.DataAnnotations;

namespace MarketTradeApplication.ViewM
{
    public class LoginVM
    {
       
        [Required(ErrorMessage = "Enter UserName")]
        public string UserName { get; set; } = default!;

        [Required(ErrorMessage = "Enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }


    }
}
