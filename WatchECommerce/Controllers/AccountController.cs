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

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View();

            var existUser = await _userManager.FindByNameAsync(model.UserName);

            if (existUser is not null)
            {
                ModelState.AddModelError("", "There is a username at the same username");
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

            var unicalFileName = await model.Image.GenerateFile(Constants.UserPath);

            var gender = "";

            if(model.Gender)
            {
                gender = "Male";
            }
            else  
            {
                gender = "Female";
            }
            var user = new User
            {
                Name = model.Name,
                LastName = model.LastName,
                BirthDate = model.BirthDate,
                Email = model.Email,
                UserName = model.UserName,
                ImageUrl = unicalFileName,
                Gender=  gender

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
            await _signInManager.SignInAsync(user,true);
            return RedirectToAction(nameof(Index), "Home");
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var existUser = await _userManager.FindByNameAsync(model.UserName);

                if (existUser == null)
                {
                    ModelState.AddModelError("", "UserName is not correct");
                    return View();
                }

                var result = await _signInManager.PasswordSignInAsync(existUser, model.Password, false, true);

                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Email must be verified");
                    return View();
                }



                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Invalid credentials");
                    return View();
                }

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

       
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
