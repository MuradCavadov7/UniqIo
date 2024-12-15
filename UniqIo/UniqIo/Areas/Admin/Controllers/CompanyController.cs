using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqIo.DAL;
using UniqIo.Extension;
using UniqIo.Helpers;
using UniqIo.Models;
using UniqIo.ViewModel.Commons;
using UniqIo.ViewModel.Companies;

namespace UniqIo.Areas.Admin.Controllers;



[Area("Admin")]
[Authorize(Roles = RoleConstants.AccessToDashboard)]
public class CompanyController(UniqIoDbContext _context) : Controller
{
    public async Task<IActionResult> Index(int? page = 1,int? take = 3)
    {
        ViewBag.Pagination = new PaginationItemsVM(await _context.Companies.CountAsync(), take.Value, page.Value);
        return View(await _context.Companies.Skip((page.Value - 1) * take.Value).Take(take.Value).ToListAsync());
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
        var data = await _context.Companies.Where(x => x.Id == id).Select(x => new CCreateVM
        {
            Name = x.Name
        }).FirstOrDefaultAsync();
        
        if (data == null) return NotFound();
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int? id, CCreateVM vm)
    {
        if (id == null) return BadRequest();
        var entity = await _context.Companies.FindAsync(id);
        if (entity == null) return NotFound();
        entity.Name = vm.Name;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
