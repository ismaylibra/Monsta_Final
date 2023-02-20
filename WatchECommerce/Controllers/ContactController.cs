using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Watch.Core.Entities;
using Watch.Core.IdentityModels;
using Watch.DAL.DAL;
using WatchECommerce.ViewModels;

namespace WatchECommerce.Controllers
{
    public class ContactController : Controller
    {
        private readonly WatchDbContext _dbContext;
        private readonly UserManager<User> _usermanager;

        public ContactController(WatchDbContext dbContext, UserManager<User> usermanager)
        {
            _dbContext = dbContext;
            _usermanager = usermanager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ContactViewModel();
            if (User.Identity.IsAuthenticated)
            {
                var user = await _usermanager.FindByNameAsync(User.Identity.Name);

                model.ContactMessage = new ViewModels.ContactMessageViewModel
                {
                    Name= user.UserName,
                    Email = user.Email,
                    ImageUrl = user.ImageUrl
                };

               
            }
            return View(model);
        }

        public async Task<IActionResult> SendMessage(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(viewName: nameof(Index), model);
            }
            var message = new ContactMessage
            {
                Name = model.ContactMessage.Name,
                Subject = model.ContactMessage.Subject,
                Email=model.ContactMessage.Email,
                Message = model.ContactMessage.Message,
                ProfileImage = model.ContactMessage.ImageUrl
            };

            await _dbContext.ContactMessages.AddAsync(message);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
