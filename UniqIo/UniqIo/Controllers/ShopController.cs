using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UniqIo.DAL;
using UniqIo.ViewModel.Baskets;
using UniqIo.ViewModel.Company;
using UniqIo.ViewModel.Products;
using UniqIo.ViewModel.Shops;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UniqIo.Controllers;

public class ShopController(UniqIoDbContext _context) : Controller
{
    public async Task<IActionResult> Index(int? catId, string amount)
    {
        var query = _context.Products.AsQueryable();
        if (catId.HasValue)
        {
            query = query.Where(x => x.CompanyId == catId);
        }
        if (amount != null)
        {
            var prices = amount.Split('-').Select(x => Convert.ToInt32(x));
            query = query.Where(y => prices.ElementAt(0) <= y.SellPrice && prices.ElementAt(1) >= y.SellPrice);
        }
        ShopVM vm = new ShopVM();
        vm.Companies = await _context.Companies
            .Where(x => !x.IsDeleted)
            .Select(x => new CompanyAndProductVM
            {
                Id = x.Id,
                Name = x.Name,
                Count = x.Products.Count
            }).ToListAsync();
        vm.Products = await query
            .Take(6)
            .Select(p => new PListItemVM
            {
                Id = p.Id,
                Name = p.Name,
                SellPrice = p.SellPrice,
                Discount = p.Discount,
                IsStock = p.PCount > 0,
                CoverImage = p.CoverImage,
            }).ToListAsync();
        vm.ProductCount = await query.CountAsync();
        return View(vm);
    }

    public async Task<IActionResult> AddBasket(int id)
    {
        var basket = getBasket();
        var item = basket.FirstOrDefault(x=> x.Id == id);
        if (item is not null)
        {
            item.Count++;
        }
        else
        {
            basket.Add(
                new BasketCookieItemVM
                {
                    Id = id,
                    Count = 1,
                });
        }
        BasketCookieItemVM vm = new BasketCookieItemVM
        {
            Id = id,
            Count = 1,
        };
        var data = JsonSerializer.Serialize(basket);
        HttpContext.Response.Cookies.Append("basket",data);
        return Ok();  
    }
    public async Task<IActionResult> GetBasket()
    {
        return Json(getBasket());
    }
    List<BasketCookieItemVM> getBasket()
    {
        string? value = HttpContext.Request.Cookies["basket"];
        if (string.IsNullOrEmpty(value)) return [];
        return JsonSerializer.Deserialize<List<BasketCookieItemVM>>(value) ?? [];
    }
}
