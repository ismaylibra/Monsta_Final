using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Watch.Core.ContactViewModels;
using Watch.Core.Entities;
using Watch.Core.IdentityModels;
using Watch.DAL.DAL;

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

                

               
            }
            return View();
        }
    }
}
