using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using ShopingOnline.Areas.Identity.Pages.Account;
using ShopingOnline.Data;
using ShopingOnline.Dto;
using ShopingOnline.Models;
using Microsoft.AspNetCore.Authorization;

namespace ShopingOnline.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]

    [Authorize(Roles = "Admin")] // User must be in Admin OR Manager role

    public class TeamsController : Controller
    {
        private readonly DbShopingOnlineContext _db;
        private readonly IWebHostEnvironment _host;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        public TeamsController(DbShopingOnlineContext db,
            IWebHostEnvironment host, UserManager<User> userManager,
             RoleManager<IdentityRole> roleManager,
             IEmailSender emailSender)
        {
            _db = db;
            _host = host;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            return View(await _db.Teammembers.ToListAsync());
        }

        public IActionResult Teammember()
        {

            return View(_db.Teammembers.ToList());
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _db.Teammembers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeamDto team)
        {
            uploadphoto(team);
            if (team.Image != null)
            {
                Team T = new Team();
                T.Email = team.Email;
                T.Date = team.Date;
                T.Image = team.Image;
                T.Jobs = team.Jobs;
                T.Name = team.Name;
               
                _db.Add(T);
                User u = new User();
                u.FullName = team.Name;
               // u.PasswordHash = password;
                u.Email = team.Email;
                u.Role = "Admin";
                u.UserName = team.Email;
                u.PhoneNumber = team.PhonNumber;
                u.Create = DateTime.Now;

                var result = await _userManager.CreateAsync(u, team.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    }
                    await _userManager.AddToRoleAsync(u, "Admin");
                   // await _userManager.AddToRoleAsync(user, Input.Role);

                    var userId = await _userManager.GetUserIdAsync(u);
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                    }
                }
                //u.PhoneNumber=team.ph
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _db.Teammembers.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Jobs,Email,Image,Date")] Team team)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(team);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Id))
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
            return View(team);
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _db.Teammembers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _db.Teammembers.FindAsync(id);
            if (team != null)
            {
                _db.Teammembers.Remove(team);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _db.Teammembers.Any(e => e.Id == id);
        }

        void uploadphoto(TeamDto model)
        {
            if (model.File != null)
            {
                string uploadfolder = Path.Combine(_host.WebRootPath, "images/Teammembers");
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
