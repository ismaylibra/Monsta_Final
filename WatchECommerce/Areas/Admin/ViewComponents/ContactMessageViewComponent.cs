using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Watch.DAL.DAL;
using WatchECommerce.Areas.Admin.ViewModels;

namespace WatchECommerce.Areas.Admin.ViewComponents
{
	public class ContactMessageViewComponent : ViewComponent
	{
        private readonly WatchDbContext _dbContext;

		public ContactMessageViewComponent(WatchDbContext dbContext)
		{
			_dbContext = dbContext;
		}

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var message = await _dbContext.ContactMessages.ToListAsync();

            var isAllReadMessage = message.All(x => x.IsRead);

            return View(new ContactMessageReadViewModel
            {
                ContactMessages = message,
                IsAllReadMessage = isAllReadMessage
            });
        }
    }
}
