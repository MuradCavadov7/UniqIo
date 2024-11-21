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
        if (!ModelState.IsValid) return View(vm);
        if (!vm.File.ContentType.IsValidType())
        {
            ModelState.AddModelError("File", "Cannot be anything else of type image");
            return View(vm);
        }
        if (vm.File.Length.IsValidSize())
        {
            ModelState.AddModelError("File", "The file size can be max 5mb");
            return View(vm);
        }
        //string newFileName = Path.GetRandomFileName() + Path.GetExtension(vm.File.FileName);
        var newFileName =vm.File.Upload(Path.Combine(_env.WebRootPath, "imgs", "sliders"));
        

        //using (Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath, "imgs", "sliders", newFileName)))
        //{
        //    await vm.File.CopyToAsync(stream);
        //}
        Slider slider = new Slider
        {
            ImageUrl = newFileName,
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
        var vm =_context.Sliders.Where(x=>x.Id == id).FirstOrDefault();
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
}
