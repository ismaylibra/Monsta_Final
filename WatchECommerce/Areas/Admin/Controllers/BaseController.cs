using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Watch.BLL.Data;

namespace WatchECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize (Roles =Constants.AdminRole)]
    public class BaseController : Controller
    {
      
    }
}
