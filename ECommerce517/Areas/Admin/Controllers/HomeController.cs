using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce517.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    [Authorize(Roles =$"{SD.SuperAdminRole},{SD.AdminRole}")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NotFoundPage()
        {
            return View();
        }
    }
}
