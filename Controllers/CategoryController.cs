using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsStore.Common;
using SportsStore.Data;
using SportsStore.Models;

namespace SportsStore.Controllers
{
    [Authorize]
    public class CategoryController : BaseController<CategoryController>
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
              return _context.Category != null ? 
                          View(await _context.Category.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Category'  is null.");
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.AUTO_ID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AUTO_ID,CategoryName,CREATED_BY,CREATED_DATE")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.CREATED_BY = CurrentUserName;
                category.CREATED_DATE = DateTime.Now;

                _context.Add(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Saved Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AUTO_ID,CategoryName,CREATED_BY,CREATED_DATE")] Category category)
        {
            if (id != category.AUTO_ID)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        category.CREATED_BY = CurrentUserName;
                        category.CREATED_DATE = DateTime.Now;

                        _context.Update(category);
                        await _context.SaveChangesAsync();

                        TempData["Success"] = "Updated Successfully";
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CategoryExists(category.AUTO_ID))
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
            }
            catch(Exception ex)
            {
                TempData["Error"] = "Somthing Went Wrong";
            }
            return View(category);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.AUTO_ID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Category == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Category'  is null.");
            }
            var category = await _context.Category.FindAsync(id);
            if (category != null)
            {
                try
                {
                    _context.Category.Remove(category);
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    TempData["Error"]="Sorry! Failed to remove. Message : "+ex.Message;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Category?.Any(e => e.AUTO_ID == id)).GetValueOrDefault();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
