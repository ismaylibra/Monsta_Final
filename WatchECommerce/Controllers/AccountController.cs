using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using Watch.BLL.Data;
using Watch.BLL.Extensions;
using Watch.BLL.Services;
using Watch.Core.IdentityModels;
using WatchECommerce.ViewModels;

namespace WatchECommerce.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMailService _mailManager;
        private readonly IConfiguration _config;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IMailService mailManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mailManager = mailManager;
            _config = config;
        }


        public IActionResult Register()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(!ModelState.IsValid) return View();

            var existUser = await _userManager.FindByNameAsync(model.UserName);

            if(existUser is not null)
            {
                ModelState.AddModelError("", "There is a username at the same username");
                return View();
            }

            var gender = "";

            if (model.Gender)
            {
                gender = "Male";
            }
            else
            {
                gender = "Female";
            }

            if (!model.Image.IsImage())
            {
                ModelState.AddModelError("Image", "An image must be selected..!");
                return View(model);
            }

            if (!model.Image.IsAllowedSize(20))
            {
                ModelState.AddModelError("Image", "Image size can be maximum 20mb..!");
                return View(model);
            }
            

            var unicalFileName = await model.Image.GenerateFile(Constants.UserImagePath);
            var user = new User
            {
                FirstName = model.Name,
                LastName = model.LastName,
                Email = model.Email,
                Gender = gender,
                BirthDate = model.BirthDate,
                UserName = model.UserName,
                Address  = model.Address,
                ImageUrl = unicalFileName

            };

            string token = Guid.NewGuid().ToString();
            user.EmailConfirmationToken = token;
            var result = await _userManager.CreateAsync(user,model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            var createdUser = await _userManager.FindByNameAsync(model.UserName);

            result = await _userManager.AddToRoleAsync(createdUser, Constants.UserRole);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            var link = Url.Action(nameof(VerifyEmail), "Account", new { id = user.Id, token }, Request.Scheme, Request.Host.ToString());

            EmailViewModel email = _config.GetSection("Email").Get<EmailViewModel>();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email.SenderEmail, email.SenderName);
            mail.To.Add(user.Email);
            mail.Subject = "VerifyEmail";
            string body = "";
            using (StreamReader reader = new StreamReader("wwwroot/assets/template/email_verify_email.html"))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{link}", link);
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = email.Server;
            smtp.Port = email.Port;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(email.SenderEmail, email.Password);
            smtp.Send(mail);
            return RedirectToAction(nameof(EmailVerification));

        }

        public IActionResult EmailVerification()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View();

            var existUser = await _userManager.FindByNameAsync(model.UserName);

            if (existUser is null)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View();
            }

            var signResult = await _signInManager.PasswordSignInAsync(existUser, model.Password, model.RememberMe, true);

            if (!signResult.Succeeded)
            {
                ModelState.AddModelError("", "Invalid credentials");

                return View();
            }

            return RedirectToAction("Index","Home");
        }
        public IActionResult Index()
        {
            return View();
        }


        
        public IActionResult AccessDenied()
        {
            return RedirectToAction("Login", "Account");
        }
       
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ChangePassword()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Login));
            };
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null) return BadRequest();
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();

            }
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }


        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            await _userManager.ConfirmEmailAsync(user, token);

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> VerifyEmail(string id, string token)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            if (user.EmailConfirmationToken != token)
            {
                return BadRequest();
            }

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);

            if (result.Succeeded)
            {
                string newToken = Guid.NewGuid().ToString();
                user.EmailConfirmationToken = newToken;
                await _userManager.UpdateAsync(user);
                return View();
            }

            return BadRequest();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Must be write email address");
                return View();
            }

            User user = await _userManager.FindByEmailAsync(model.Mail);

            if (user is null)
            {
                ModelState.AddModelError("", "So the email is not available");
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var link = Url.Action(nameof(ResetPassword), "Account", new { id = user.Id, token }, Request.Scheme, Request.Host.ToString());

            EmailViewModel email = _config.GetSection("Email").Get<EmailViewModel>();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email.SenderEmail, email.SenderName);
            mail.To.Add(model.Mail);
            mail.Subject = "Reset Password";
            mail.Body = $"<a href=\"{link}\">Reset Password</a>";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = email.Server;
            smtp.Port = email.Port;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(email.SenderEmail, email.Password);
            smtp.Send(mail);

            return RedirectToAction(nameof(EmailVerification));
        }
        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Must be filled in correctly");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null) return BadRequest();

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);

            }

            return View();
        }

    }
}
