using Microsoft.AspNetCore.Mvc;
using WatchECommerce.ViewModels;

namespace WatchECommerce.Controllers
{
    public class ErrorPageController : Controller
    {
        public IActionResult ErrorAction(int statusCode)

        {
            ErrorViewModel error = new ErrorViewModel()
            {
                StatusCode = HttpContext.Response.StatusCode,
                Title = HttpContext.Response.Headers.ToString()
            };
            return View();
        }
    }
}
