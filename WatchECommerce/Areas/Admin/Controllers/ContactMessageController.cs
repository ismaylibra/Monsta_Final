using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.DAL.DAL;
using WatchECommerce.Areas.Admin.ViewModels;

namespace WatchECommerce.Areas.Admin.Controllers
{
    public class ContactMessageController : BaseController
    {
        private readonly WatchDbContext _dbContext;

        public ContactMessageController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Read(int? id)
        {
            if (id is null) return BadRequest();

            var message = _dbContext.ContactMessages.FirstOrDefault(x => x.Id == id);

            if (message is null) return NotFound();

            if (!message.IsRead)
            {
                message.IsRead = true;
                await _dbContext.SaveChangesAsync();
            }

            ContactMessageReadViewModel model = new ContactMessageReadViewModel()
            {
                ContactMessage = message,
                IsAllReadMessage = await _dbContext.ContactMessages.AnyAsync(m => !m.IsRead && !m.IsDeleted)
            };

            return View(model);
        }
    }
}
