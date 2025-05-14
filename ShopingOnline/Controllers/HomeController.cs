using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopingOnline.Models;

namespace ShopingOnline.Controllers
{

    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private DbShopingOnlineContext db;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, DbShopingOnlineContext _context, UserManager<User> userManager)
        {
            _logger = logger;
            db = _context;
        }
    

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Product(int id)
        {
           
            TempData["CategoryId"] = id;
            var result = db.Products.Where(x => x.CategoryId == id).ToList();
            var userId = HttpContext.Session.GetString("UserId");
            var cartitemscount = db.CartItems.Where(x => x.UserId == userId).Sum(x => x.Quantity);
            ViewBag.CartItemsCount = cartitemscount;

            return View(result);
        }

        public IActionResult Categories()
        {

            return View(db.Categories.ToList());

        }
        [HttpGet]
        [Authorize]

        public IActionResult AddToCart(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            var username = HttpContext.Session.GetString("UserName");
            var userId = HttpContext.Session.GetString("UserId");

            var existingCartItem = db.CartItems.FirstOrDefault(ci => ci.ProductId == id && ci.User.UserName==username && ci.UserId == userId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity++;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = id,
                    Quantity = 1
                };
                db.CartItems.Add(cartItem);
            }

            db.SaveChanges();

            // «” —Ã⁄ categoryId „‰ TempData
            int categoryId = Convert.ToInt32(TempData["CategoryId"]);

            // √⁄œ  Œ“Ì‰Â ·√‰ TempData Ìı” Â·ﬂ „—… Ê«Õœ…
            TempData["CategoryId"] = categoryId;

            // —ÃÊ⁄ ·‰›” ’›Õ… «·„‰ Ã«  Õ”» «·’‰›
            return RedirectToAction("Product", new { id = categoryId });
        }
        public IActionResult HotalProduct()
        {
            var result = db.Products.Include(p => p.Category).ToList(); // <<< ÷—Ê—ÌToList();

            return View(result);
        }
        //http Get
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(Contact model)
        {
            if (ModelState.IsValid)
            {
                db.Contacts.Add(model);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View("Contact");
        }
        public IActionResult Messages()
        {
            var result = db.Contacts.ToList();
            if (result == null)
                return NotFound();

            return View(result);
        }
        // ⁄—÷  ›«’Ì· «·„‰ Ã „⁄ «· ⁄·Ìﬁ« 
        public IActionResult ProductDetails(int id)
        {
            var product = db.Products
                .Include(p => p.Comments)  // «” Œœ«„ Include · Õ„Ì· «· ⁄·Ìﬁ«  «·„— »ÿ… „»«‘—…
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductCommentsViewModel
            {
                Product = product,
                Comments = product.Comments.ToList()  // «·Õ’Ê· ⁄·Ï «· ⁄·Ìﬁ«  «·„— »ÿ… »«·„‰ Ã
            };

            return View(viewModel);
        }

        // ≈÷«›…  ⁄·Ìﬁ ÃœÌœ (≈–« ﬂ«‰ «·„” Œœ„ „”Ã· «·œŒÊ·)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(ProductCommentsViewModel model, int productId)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var newComment = new Comment
                {
                    UserId = user.Id,
                    ProductId = productId,
                    CommentText = model.NewCommentText,
                    Stars = model.NewStars,
                    CreatedAt = DateTime.Now
                };

                db.Comments.Add(newComment);
                await db.SaveChangesAsync();

                // ≈⁄«œ…  ÊÃÌÂ ≈·Ï ’›Õ… «·„‰ Ã „⁄ «· ⁄·Ìﬁ« 
                return RedirectToAction("ProductDetails", new { id = productId });  //  ÕœÌÀ «·„⁄«„· ≈·Ï "id"
            }

            // ≈–« ﬂ«‰  «·»Ì«‰«  €Ì— ’«·Õ…° ≈⁄«œ… ‰›” «·’›Õ…
            var product = db.Products
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.Id == productId);

            model.Product = product;
            model.Comments = product?.Comments.ToList() ?? new List<Comment>();  // «· √ﬂœ „‰ ⁄œ„ «·Õ’Ê· ⁄·Ï ﬁÌ„… ›«—€…

            return View("ProductDetails", model);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
