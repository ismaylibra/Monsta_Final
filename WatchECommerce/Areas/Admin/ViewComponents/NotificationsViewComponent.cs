using Microsoft.AspNetCore.Mvc;

namespace WatchECommerce.Areas.Admin.ViewComponents
{
    public class NotificationsViewComponent : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
