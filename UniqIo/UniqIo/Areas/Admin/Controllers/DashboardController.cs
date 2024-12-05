using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniqIo.Helpers;

namespace UniqIo.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        [Area("Admin")]
        [Authorize(Roles = RoleConstants.AccessToDashboard)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
