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

    public class ProductsController : Controller
    {
        private readonly DbShopingOnlineContext _db;
        private readonly IWebHostEnvironment _host;
        public ProductsController(DbShopingOnlineContext db, IWebHostEnvironment host)
        {
            _db = db;
            _host = host;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var dbShopingOnlineContext = _db.Products.Include(p => p.Category);
            return View(await dbShopingOnlineContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_db.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (product.Image == null)
            {
                ModelState.AddModelError("Image", "برجاء إضافة صورة للمنتج");
            }


            try
            {
                uploadphoto(product);
                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "حدث خطأ أثناء حفظ البيانات.");
            }


            ViewData["CategoryId"] = new SelectList(_db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {

            {
                try
                {
                    var productEdit = _db.Products.FirstOrDefault(x => x.Id == product.Id);
                    if (productEdit == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        productEdit.Name = product.Name;
                        productEdit.Description = product.Description;
                        productEdit.Price = product.Price;
                        productEdit.Image = product.Image;
                        productEdit.Quantity = product.Quantity;
                        productEdit.CategoryId = product.CategoryId;
                        _db.Products.UpdateRange();
                        await _db.SaveChangesAsync();

                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                _db.Products.Remove(product);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _db.Products.Any(e => e.Id == id);
        }
        void uploadphoto(Product model)
        {
            if (model.File != null)
            {
                string uploadfolder = Path.Combine(_host.WebRootPath, "images/Products");
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
