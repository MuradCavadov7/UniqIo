using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using UniqIo.DAL;
using UniqIo.ViewModel.Baskets;
using UniqIo.ViewModel.Companies;
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
        var item = basket.FirstOrDefault(x => x.Id == id);
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
        HttpContext.Response.Cookies.Append("basket", data);
        return RedirectToAction("Index","Home");
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var data = await _context.Products
            .Include(x => x.Images)
            .Include(x => x.ProductRatings)
            .Include(x=>x.ProductComments)
            .Where(x => x.Id == id.Value && !x.IsDeleted).FirstOrDefaultAsync();
        if (data is null) return NotFound();
        string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId is not null)
        {
            var rating = await _context.ProductRatings.Where(x => x.UserId == userId && x.ProductId == id).Select(x => x.RatingRate).FirstOrDefaultAsync();
            ViewBag.Rating = rating == 0 ? 5 : rating;
        }
        else
        {
            ViewBag.Rating = 5;
        }
        return View(data);
    }

    [Authorize]
    public async Task<IActionResult> Rate(int? productId, int rate = 1)
    {
        if (productId == null) return BadRequest();
        string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (!await _context.Products.AnyAsync(x => x.Id == productId)) return NotFound();
        var rating = await _context.ProductRatings.Where(r => r.ProductId == productId && r.UserId == userId)
            .FirstOrDefaultAsync();
        if (rating is null)
        {
            await _context.ProductRatings.AddAsync(new Models.ProductRating
            {
                ProductId = productId,
                RatingRate = rate,
                UserId = userId
            });
        }
        else
        {
            rating.RatingRate = rate;
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = productId });
    }

  
    [Authorize]
    public async Task<IActionResult> Comment(int? productId, string comment)
    {
        if (productId is null) return BadRequest();
        string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (!await _context.Products.AnyAsync(x => x.Id == productId)) return NotFound();
        var commit = await _context.ProductComments.Where(r => r.ProductId == productId && r.UserId == userId)
            .FirstOrDefaultAsync();
        if (commit is null)
        {
            await _context.ProductComments.AddAsync(new Models.ProductComment
            {
                ProductId = productId,
                CommitComment = comment,
                UserId = userId
            });
        }
        else
        {
            commit.CommitComment = comment.Trim();
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = productId });
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

    public async Task<IActionResult> DeleteBasketItem(int id)
    {
        var basket = getBasket();
        var item = basket.FirstOrDefault(x => x.Id == id);
        if (item is not null)
        {
            item.Count--;

            if (item.Count == 0)
            {
                basket.Remove(item);
            }
            var data = JsonSerializer.Serialize(basket);
            HttpContext.Response.Cookies.Append("basket", data);
        }
        return RedirectToAction("Index","Home");
    }
}
