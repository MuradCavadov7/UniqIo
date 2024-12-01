using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UniqIo.DAL;
using UniqIo.Models;
using UniqIo.ViewModel.Commons;
using UniqIo.ViewModel.Products;
using UniqIo.ViewModel.Sliders;

namespace UniqIo.Controllers
{
    public class HomeController(UniqIoDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            HomeVM vm = new HomeVM();
            vm.Sliders = await _context.Sliders.Select(s => new SListItemVM
            {
                ImageUrl = s.ImageUrl,
                Link = s.Link,
                Title = s.Title,
                Subtitle = s.Subtitle
            }).ToListAsync();
            vm.Companies = await _context.Companies.OrderByDescending(x => x.Products!.Count).Take(4).ToListAsync();
            vm.PopularProducts = await _context.Products.Where(x => vm.Companies.Select(y => y.Id).Contains(x.CompanyId!.Value)).Take(10)
                .Select(p => new PListItemVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    SellPrice = p.SellPrice,
                    Discount = p.Discount,
                    IsStock = p.PCount > 0,
                    CoverImage = p.CoverImage,
                    CompanyId = p.CompanyId.Value
                }).ToListAsync();

            return View(vm);
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
