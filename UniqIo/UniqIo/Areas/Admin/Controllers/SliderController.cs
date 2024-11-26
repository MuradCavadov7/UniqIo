using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Net;
using UniqIo.DAL;
using UniqIo.Extention;
using UniqIo.Models;
using UniqIo.ViewModel.Sliders;

namespace UniqIo.Areas.Admin.Controllers;


[Area("Admin")]
public class SliderController(UniqIoDbContext _context, IWebHostEnvironment _env) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _context.Sliders.ToListAsync());
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(SCreateVM vm)
    {
        if (vm.File != null)
        {
            if (!vm.File.IsValidType("image"))
            {
                ModelState.AddModelError("File", "Cannot be anything else of type image");
            }
            if (!vm.File.IsValidSize(800))
            {
                ModelState.AddModelError("File", "The file size can be max 800kb");
            }
        }

        if (!ModelState.IsValid) return View(vm);
        Slider slider = new Slider
        {
            ImageUrl = await vm.File!.UploadAsync(_env.WebRootPath, "imgs", "sliders"),
            Title = vm.Title,
            Subtitle = vm.Subtitle!,
            Link = vm.Link
        };
        await _context.Sliders.AddAsync(slider);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();
        var vm = _context.Sliders.Where(x => x.Id == id).FirstOrDefault();
        string imagePath = Path.Combine(_env.WebRootPath, "imgs", "sliders", vm.ImageUrl);
        if (System.IO.File.Exists(imagePath))
        {
            System.IO.File.Delete(imagePath);
        }
        if (await _context.Sliders.AnyAsync(x => x.Id == id))
        {
            _context.Sliders.Remove(vm);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id == null)
            return BadRequest();

        var slider = await _context.Sliders
                .Where(x => x.Id == id)
                .Select(x => new SUpdateVM
                {
                    Title = x.Title,
                    Link = x.Link,
                    Subtitle = x.Subtitle,
                    FileUrl = x.ImageUrl
                })
                .FirstOrDefaultAsync();
        if (slider == null)
            return NotFound();
        return View(slider);
    }


    [HttpPost]
    public async Task<IActionResult> Update(int? id, SUpdateVM vm)
    {
        if (id == null) return BadRequest();
        if (vm.File != null)
        {
            if (!vm.File.IsValidType("image"))
            {
                ModelState.AddModelError("File", "Cannot be anything else of type image");
            }
            if (!vm.File.IsValidSize(800))
            {
                ModelState.AddModelError("File", "The file size can be max 800kb");
            }
        }
        if (!ModelState.IsValid) return View(vm);
        var entity = await _context.Sliders.FindAsync(id);
        if (entity == null) return NotFound();
        entity.Title = vm.Title;
        entity.Link = vm.Link;
        entity.Subtitle = vm.Subtitle;
        entity.ImageUrl = await vm.File.UploadAsync(_env.WebRootPath, "imgs", "sliders");
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Hide(int? id)
    {
        var slider = await _context.Sliders.FindAsync(id);
        if (slider == null) return NotFound();
        slider.IsDeleted = true;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Show(int? id)
    {

        var slider = await _context.Sliders.FindAsync(id);
        if (slider == null) return NotFound();
        slider.IsDeleted = false;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
