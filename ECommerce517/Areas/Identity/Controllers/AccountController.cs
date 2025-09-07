using ECommerce517.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;



namespace ECommerce517.Areas.Identity.Controllers
{
    [Area(SD.IdentityArea)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRepository<UserOTP> _userOTP;

        public AccountController(UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            SignInManager<ApplicationUser> signInManager,
            IRepository<UserOTP> userOTP)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _userOTP = userOTP;
        }


        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            ApplicationUser user = new ApplicationUser()
            {
                Name = registerVM.Name,
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                Street = registerVM.Street,
                City = registerVM.City,
                State = registerVM.State,
                ZipCode = registerVM.ZipCode
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
                return View(registerVM);
            }

            //  add role user => customer

            await _userManager.AddToRoleAsync(user, SD.CustomerRole);

            //confirm message 

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action("ConfirmEmail", "Account", new { area = "Identity", token = token, userId = user.Id }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confim your Email", $"<h1> Confirm your  email by clicking  <a target='_blank' href='{link}'>Here</a> </h1>");
            TempData["success-notification"] = "User created Successfully,check your inbox to confirm";

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                TempData["error-notification"] = " Expired!, resend confirmation";
            else
                TempData["success-notification"] = "Confirmed Successfully";

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var user = await _userManager.FindByEmailAsync(login.UserNameOrEmail) ?? await _userManager.FindByNameAsync(login.UserNameOrEmail);

            if (user is null)
            {
                TempData["error-notification"] = "Username or password is invalid";
                return View(login);
            }

            var result = await _signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, true);

            if (!result.Succeeded)
            {


                TempData["error-notification"] = "Username or password is invalid ,faild";
                return View(login);
            }

            if (!user.EmailConfirmed)
            {
                TempData["error-notification"] = "Email Not Confirmed";
                return View(login);
            }


            TempData["success-notification"] = "Login Successfully";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmation)
        {
            var user = await _userManager.FindByEmailAsync(resendEmailConfirmation.UserNameOrEmail) ?? await _userManager.FindByNameAsync(resendEmailConfirmation.UserNameOrEmail);

            if (user is null)
            {
                TempData["error-notification"] = "Username or password is invalid ,notfound";
                return View(resendEmailConfirmation);
            }
            if (user.EmailConfirmed)
            {
                TempData["error-notification"] = "Username Already confirmed";
                return View(resendEmailConfirmation);
            }

            // send => confirm message

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("ConfirmEmail", "Account", new { area = "Identity", token, userId = user.Id }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email!, "Confirm Your Email", $"<h1> Confirm your  email by clicking  <a target='_blank' href='{link}'>Here</a> </h1>");
            TempData["success-notification"] = "Check inbox to confirm";
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(e => e.Errors.Select(e => e.ErrorMessage));

                TempData["error-notification"] = String.Join(", ", errors);

                return View(forgetPasswordVM);
            }

            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail) ??
                       await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail);

            if (user is null)
            {
                TempData["error-notification"] = "Invalid UserName or Email !";
                return View();
            }

            // create ==> otp and send it 

            var otpNumber = new Random().Next(1000, 9999);
            await _userOTP.CreateAsync(new UserOTP
            {
                ApplicationUserId = user.Id,
                OTPNumber = otpNumber,
                ValidTo = DateTime.UtcNow.AddDays(1)
            });

            await _userOTP.CommitAsync();


            await _emailSender.SendEmailAsync(user.Email!, "ResetPassword", $"<h1> Reset OTP Number is <strong style='color:indigo'>{otpNumber} </strong></h1>");
            TempData["success-notification"] = "Check you inbox for OTP number";
            return RedirectToAction("ResetPassword", "Account", new { area = "Identity", userId = user.Id });

        }

        [HttpGet]
        public IActionResult ResetPassword(string userId)
        {
            return View(new ResetPasswordVM
            {
                ApplicationUserId = userId,
            });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordVM);
            }

            var user = await _userManager.FindByIdAsync(resetPasswordVM.ApplicationUserId);

            if (user is null)
            {
                TempData["error-notification"] = "Error with OTP User Data!";
                return View(resetPasswordVM);

            }

            var userOTP = (await _userOTP.GetAsync(e => e.ApplicationUserId == user.Id)).OrderBy(e => e.Id).LastOrDefault();

            if (resetPasswordVM.OTPNumber != userOTP?.OTPNumber)
            {
                TempData["error-notification"] = "Wrong OTP Number !!";
                return View(resetPasswordVM);
            }

            if (DateTime.UtcNow > userOTP.ValidTo)
            {

                TempData["error-notification"] = "Expired OTP resend !";
                return View(resetPasswordVM);
            }

            TempData["success-notification"] = " valid OTP number";

            return RedirectToAction("ChangePassword", "Account", new { area = "Identity", userId = user.Id });
        }

        [HttpGet]
        public IActionResult ChangePassword(string userId)
        {
            return View(new ChangePasswordVM
            {
                ApplicationUserId = userId,
            });
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM changePasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordVM);

            }

            var user = await _userManager.FindByIdAsync(changePasswordVM.ApplicationUserId);

            if (user is null)
            {
                TempData["error-notification"] = "invalid user name!!";
                return View(changePasswordVM);
            }

            var temptoken = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _userManager.ResetPasswordAsync(user, temptoken, changePasswordVM.Password);

            TempData["success-notification"] = "Password Changed successfully";

            return RedirectToAction("Login", "Account", new { area = "Identity" });

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }
    }
}
