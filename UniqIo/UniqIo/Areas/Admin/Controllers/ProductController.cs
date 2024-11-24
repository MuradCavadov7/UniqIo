using Microsoft.AspNetCore.Mvc;
using static UniqIo.ViewModel.Products.PCreateVM;
using UniqIo.DAL;
using UniqIo.Models;
using UniqIo.Extention;
using Microsoft.EntityFrameworkCore;
using UniqIo.ViewModel.Products;
using UniqIo.ViewModel.Sliders;

namespace UniqIo.Areas.Admin.Controllers;


[Area("Admin")]
public class ProductController(IWebHostEnvironment _env, UniqIoDbContext _context) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _context.Products.Include(x=>x.Company).ToListAsync());
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
        if (vm.OtherFiles.Any())
        {
            if (!vm.OtherFiles.All(x => x.IsValidType("image")))
            {
                string fileNames = string.Join(',', vm.OtherFiles.Where(x => !x.IsValidType("image")).Select(x => x.FileName));
                ModelState.AddModelError("OtherFiles", fileNames + "File(s) must be an image");
            }
            if (!vm.OtherFiles.All(x => x.IsValidSize(800)))
            {
                string fileNames = string.Join(',', vm.OtherFiles.Where(x => !x.IsValidSize(800)).Select(x => x.FileName));
                ModelState.AddModelError("OtherFiles", fileNames + "File(s) must be less than 800kb");
            }

        }
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Companies.Where(x => !x.IsDeleted).ToListAsync();
            return View(vm);
        }
        if (!await _context.Companies.AnyAsync(x => x.Id == vm.CompanyId))
        {
            ViewBag.Categories = await _context.Companies.Where(x => !x.IsDeleted).ToListAsync();
            ModelState.AddModelError("CompanyId", "Company not found");
            return View();
        }
        Product product = vm;
        product.CoverImage = await vm.File!.UploadAsync(_env.WebRootPath, "imgs", "products");
        product.Images = vm.OtherFiles.Select(x => new ProductImage
        {
            ImageUrl = x.UploadAsync(_env.WebRootPath, "imgs", "products").Result
        }).ToList();
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();
        var vm = _context.Products.Where(x => x.Id == id).FirstOrDefault();
        string imagePath = Path.Combine(_env.WebRootPath, "imgs", "products", vm.CoverImage);
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
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();
        ViewBag.Categories = await _context.Companies.Where(x => !x.IsDeleted).ToListAsync();
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
        entity.Images = product.Images;
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
