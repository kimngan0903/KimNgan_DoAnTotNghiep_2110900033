using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlantCare.Models;

namespace PlantCare.Areas.AdminQL.Controllers
{
    public class ServiceCategoriesController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public ServiceCategoriesController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/ServiceCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.ServiceCategories.ToListAsync());
        }

        // GET: AdminQL/ServiceCategories/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var serviceCategory = await _context.ServiceCategories.FindAsync(id);
            if (serviceCategory == null)
            {
                return NotFound();
            }
            return PartialView("_Details", serviceCategory);
        }

        // GET: AdminQL/ServiceCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminQL/ServiceCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryServiceId,CategoryServiceName")] ServiceCategory serviceCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviceCategory);
        }

        // GET: AdminQL/ServiceCategories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var serviceCategory = await _context.ServiceCategories.FindAsync(id);
            if (serviceCategory == null)
            {
                return NotFound();
            }
            return PartialView("_Edit", serviceCategory);
        }

        // POST: AdminQL/ServiceCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryServiceId,CategoryServiceName")] ServiceCategory serviceCategory)
        {
            if (id != serviceCategory.CategoryServiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceCategoryExists(serviceCategory.CategoryServiceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(serviceCategory);
        }

        // GET: AdminQL/ServiceCategories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var serviceCategory = await _context.ServiceCategories.FindAsync(id);
            if (serviceCategory == null)
            {
                return NotFound();
            }
            return PartialView("_Delete", serviceCategory);
        }

        // POST: AdminQL/ServiceCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceCategory = await _context.ServiceCategories.FindAsync(id);
            if (serviceCategory != null)
            {
                _context.ServiceCategories.Remove(serviceCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceCategoryExists(int id)
        {
            return _context.ServiceCategories.Any(e => e.CategoryServiceId == id);
        }
    }
}
