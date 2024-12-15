using Microsoft.AspNetCore.Mvc;
using static UniqIo.ViewModel.Products.PCreateVM;
using UniqIo.DAL;
using UniqIo.Models;
using UniqIo.Extention;
using Microsoft.EntityFrameworkCore;
using UniqIo.ViewModel.Products;
using UniqIo.ViewModel.Sliders;
using static System.Runtime.InteropServices.JavaScript.JSType;
using NuGet.Packaging;
using Microsoft.AspNetCore.Authorization;
using UniqIo.Helpers;
using UniqIo.ViewModel.Commons;

namespace UniqIo.Areas.Admin.Controllers;


[Area("Admin")]
[Authorize(Roles = RoleConstants.AccessToDashboard)]
public class ProductController(IWebHostEnvironment _env, UniqIoDbContext _context) : Controller
{
    public async Task<IActionResult> Index(int? take = 2, int? page = 1 )
    {
		if (!page.HasValue) page = 1;
		if (!take.HasValue) take = 2;
		var query = _context.Products.Include(x => x.Company).AsQueryable();
		var data = await query.Skip(take.Value * (page.Value - 1)).Take(take.Value).ToListAsync();
		int total = await query.CountAsync();
		ViewBag.PaginationItems = new PaginationItemsVM(total, take.Value, page.Value);
		return View(data); ;
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
        var product = await _context.Products.Where(x=>x.Id== id).Select(x=>new PUpdateVM
        {
            Id = x.Id,
            CompanyId =x.CompanyId ?? 0,
            Name = x.Name,
            Description = x.Description,
            CostPrice = x.CostPrice,
            SellPrice = x.SellPrice,
            Discount = x.Discount,
            FileUrl = x.CoverImage,
            PCount = x.PCount,
            OtherFilesUrls = x.Images.Select(y => y.ImageUrl)
        }).FirstOrDefaultAsync();
        if (product == null)
            return NotFound();
        ViewBag.Categories = await _context.Companies.Where(x => !x.IsDeleted).ToListAsync();
        return View(product);
    }


    [HttpPost]
    public async Task<IActionResult> Update(int? id,PUpdateVM vm)
    {
        if (id == null) return BadRequest();

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
        var entity = await _context.Products.FindAsync(id.Value);
        if (entity == null) return NotFound();
        entity.Name = vm.Name;
        entity.Description = vm.Description;
        entity.CostPrice = vm.CostPrice;
        entity.SellPrice = vm.SellPrice;
        entity.PCount = vm.PCount;
        entity.Discount = vm.Discount;
        entity.CompanyId = vm.CompanyId;
        entity.CoverImage = await vm.File.UploadAsync(_env.WebRootPath, "imgs", "sliders");
        entity = await _context.Products.Include(x => x.Images)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        entity.Images.AddRange(vm.OtherFiles.Select(x => new ProductImage
        {
            ImageUrl = x.UploadAsync(_env.WebRootPath, "imgs", "products").Result
        }).ToList());
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    public async Task<IActionResult> DeleteImgs(int id, IEnumerable<string> imgNames)
    {
        int result = await _context.ProductImages.Where(x => imgNames.Contains(x.ImageUrl)).ExecuteDeleteAsync();
        if (result > 0)
        {
            var stringPath = imgNames.Select(imgs => Path.Combine(_env.WebRootPath, "imgs", "products")).ToList();

            foreach (var item in stringPath)
            {
                if (System.IO.File.Exists(item))
                {
                    System.IO.File.Delete(item);
                }
            }
        }
        return RedirectToAction(nameof(Update), new { id });
    }
}
