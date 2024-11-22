using Microsoft.AspNetCore.Mvc;
using static UniqIo.ViewModel.Product.PCreateVM;
using UniqIo.DAL;
using UniqIo.Models;
using UniqIo.Extention;
using Microsoft.EntityFrameworkCore;
using UniqIo.ViewModel.Product;
using UniqIo.ViewModel.Sliders;
namespace UniqIo.Areas.Admin.Controllers;


    [Area("Admin")]
    public class ProductController(IWebHostEnvironment _env, UniqIoDbContext _context) : Controller
    {
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync();
        if (products == null)
        {
            products = new List<Product>();
        }
        return View(products);
    }
    public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Companies.Where(x => !x.IsDeleted).ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(PCreateVM vm)
        {
            if (vm.File != null)
            {
                if (!vm.File.IsValidType("image"))
                    ModelState.AddModelError("File", "File must be an image");
                if (!vm.File.IsValidSize(800))
                    ModelState.AddModelError("File", "File must be less than 800kb");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Companies.Where(x => !x.IsDeleted).ToListAsync();
                return View(vm);
            }
            if (!await _context.Products.AnyAsync(x => x.Id == vm.CompanyId))
            {
                ViewBag.Categories = await _context.Companies.Where(x => !x.IsDeleted).ToListAsync();
                ModelState.AddModelError("CompanyId", "Company not found");
                return View();
            }
            Product product = new()
            {
                CompanyId = vm.CompanyId,
                CostPrice = vm.CostPrice,
                Description = vm.Description,
                Discount = vm.Discount,
                Name = vm.Name,
                PCount = vm.PCount,
                SellPrice = vm.SellPrice
            };
            product.CoverImage = await vm.File!.UploadAsync(_env.WebRootPath, "imgs", "products");
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();
        var vm = _context.Products.Where(x => x.Id == id).FirstOrDefault();
        string imagePath = Path.Combine(_env.WebRootPath, "imgs", "sliders", vm.CoverImage);
        if (System.IO.File.Exists(imagePath))
        {
            System.IO.File.Delete(imagePath);
        }
        if (await _context.Products.AnyAsync(x => x.Id == id))
        {
            _context.Products.Remove(vm);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id == null)
            return BadRequest();

        var slider = await _context.Sliders.FindAsync(id);
        if (slider == null)
            return NotFound();



        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Update(int? id, Product product, PCreateVM vm)
    {
        if (id == null) return BadRequest();
        var entity = await _context.Products.FindAsync(id.Value);
        if (entity == null) return NotFound();
        entity.Name = product.Name;
        entity.Description = product.Description;
        entity.CostPrice = product.CostPrice;
        entity.SellPrice = product.SellPrice;
        entity.PCount = product.PCount;
        entity.Discount = product.Discount;
        entity.CompanyId = product.CompanyId;
        entity.CoverImage = product.CoverImage;
        if (vm.File is not null)
        {
            string newFileName = await vm.File.UploadAsync(_env.WebRootPath, "imgs", "sliders");
            if (!string.IsNullOrEmpty(entity.CoverImage))
            {
                string filePath = Path.Combine(_env.WebRootPath, "imgs", "sliders", entity.CoverImage);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            entity.CoverImage = newFileName;
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
