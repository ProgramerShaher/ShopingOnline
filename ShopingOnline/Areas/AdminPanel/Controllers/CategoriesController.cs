using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ShopingOnline.Data;
using ShopingOnline.Models;

namespace ShopingOnline.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class CategoriesController : Controller
    {
        
        private readonly DbShopingOnlineContext _db;
        private readonly IWebHostEnvironment _host;
        public CategoriesController(DbShopingOnlineContext db, IWebHostEnvironment host)
        {
            _db = db;
            _host = host;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _db.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _db.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {

            try
            {
                uploadphoto(category);
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                throw;
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _db.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Image")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }


            try
            {
                _db.Update(category);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _db.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category != null)
            {
                _db.Categories.Remove(category);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _db.Categories.Any(e => e.Id == id);
        }
        void uploadphoto(Category model)
        {
            if (model.File != null)
            {
                string uploadfolder = Path.Combine(_host.WebRootPath, "images/Catogry");
                string uniqefilename = Guid.NewGuid().ToString() + ".jpg";
                string filepath = Path.Combine(uploadfolder, uniqefilename);
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    model.File.CopyTo(fileStream);
                }
                model.Image = uniqefilename;
            }
        }
    }

}
