using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.DAL.DAL;

namespace WatchECommerce.Areas.Admin.ViewComponents
{
    public class UserListViewComponent : ViewComponent
    {
        private readonly WatchDbContext _dbContext;

        public UserListViewComponent(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var users = await _dbContext.Users.OrderByDescending(x => x.Id).ToListAsync();

           
            return View(users);
        }
    }
}
