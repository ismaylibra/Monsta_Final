using Microsoft.AspNetCore.Mvc;

namespace WatchECommerce.Areas.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
