using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqIo.DAL;
using UniqIo.Models;
using UniqIo.ViewModel.Company;

namespace UniqIo.Areas.Admin.Controllers;



[Area("Admin")]
public class CompanyController(UniqIoDbContext _context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var companies = await _context.Companies.ToListAsync();
        return View(companies);
    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CCreateVM vm)
    {
        if (!ModelState.IsValid) return View(vm);
        Company company = new Company();
        company.Name = vm.Name;
        _context.Add(company);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));

    }


    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();
        if (await _context.Companies.AnyAsync(x => x.Id == id))
        {
            _context.Companies.Remove(new Company { Id = id.Value });
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Update(int? id)
    {
        if (id == null) return BadRequest();
        var data = await _context.Companies.Where(x => x.Id == id.Value).FirstOrDefaultAsync();
        if (data == null) return NotFound();
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int? id, CCreateVM vm)
    {
        if (id == null) return BadRequest();
        var entity = await _context.Companies.FindAsync(id.Value);
        if (entity == null) return NotFound();
        entity.Name = vm.Name;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
