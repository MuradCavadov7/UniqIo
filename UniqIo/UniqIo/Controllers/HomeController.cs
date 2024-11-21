using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UniqIo.DAL;
using UniqIo.Models;

namespace UniqIo.Controllers
{
	public class HomeController(UniqIoDbContext _context) : Controller
	{
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sliders.ToListAsync());
        }
        public IActionResult About()
		{
			return View();
		}

        public IActionResult Contact()
        {
            return View();
        }

    }
}
