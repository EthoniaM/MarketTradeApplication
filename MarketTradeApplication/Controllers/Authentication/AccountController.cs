using MarketTradeApplication.Data;
using MarketTradeApplication.Repositories.Interface;
using MarketTradeApplication.ViewM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MarketTradeApplication.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async  Task<IActionResult> Register(RegisterVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var chkEmail = await userManager.FindByEmailAsync(model.Email);
                    if (chkEmail != null)
                    {
                        ModelState.AddModelError(string.Empty, "Email already exist");
                        return View(model);

                    }
                    var user = new ApplicationUser
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        UserName = model.Email,
                        Email = model.Email
                    };
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        bool status = await emailSender.EmailSendAsync(model.Email, "Account Created", "Your Account has been created successfully");
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    if (result.Errors.Count() > 0)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
            return View(model);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser checkEmail = await userManager.FindByEmailAsync(model.UserName);
                    if(checkEmail == null)
                    {
                        ModelState.AddModelError(string.Empty, "Email not found");
                        return View(model);
                    }
                    if (await userManager.CheckPasswordAsync(checkEmail, model.Password) == false)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid");
                        return View(model);
                    }
                    var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe,
                        lockoutOnFailure: false);
                    if(result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError(string.Empty, "Invalid Login");
                }
            }
            catch(Exception)
            {
                throw;
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgot)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Token");
            if (!ModelState.IsValid)
            {
                return View(forgot);
            }
            var user = await userManager.FindByEmailAsync(forgot.Email);
            if (user != null)
            {
                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { UserId = user.Id, Token = code },
                protocol: Request.Scheme);
                bool isSendEmail = await emailSender.EmailSendAsync(forgot.Email, "ResetPassword", "Please reset your password by clicking <a style='background-color:#0000ff; border:none; color:white; padding:10px; text-align:center; text-decoration:none; display:inline-block; font-size:16px; margin:4px 2px; cursor:pointer; border-radius:10px;' href=\"" + callbackUrl+"\">Click Here</a>");
                if (isSendEmail) 
                {
                    Response response = new Response();
                    response.Message = "Reset Password Link";
                    return RedirectToAction("ForgotPasswordConfirmation", "Account", response);
                }
            }
            return View();
        }
        public IActionResult ForgotPasswordConfirmation(Response response)
        {
            return View(response);
        }
        public IActionResult ResetPassword(string userId, string Token)
        {
            var model = new ForgotPasswordVM
            {
                Token = Token,
                UserId = userId,
            };
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ForgotPasswordVM forgot)
        {
            Response response = new Response();
            ModelState.Remove("Email");
            if (!ModelState.IsValid)
            {
                return View(forgot);
            }
            var user = await userManager.FindByIdAsync(forgot.UserId);
            if(user == null)
            {
                return View(forgot);
            }
            var result = await userManager.ResetPasswordAsync(user, forgot.Token, forgot.Password);
            if (result.Succeeded)
            {
                response.Message = "Your password has been succefuly Reset ";
                return RedirectToAction("ForgotPasswordConfirmation", response);
                    
            }
            return View(forgot);
        }
    }
}
