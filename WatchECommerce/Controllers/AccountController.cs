using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Watch.BLL.Data;
using Watch.BLL.Extensions;
using Watch.Core.IdentityModels;
using WatchECommerce.ViewModels;

namespace WatchECommerce.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
            return RedirectToAction(nameof(Login));

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
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
    }
}
