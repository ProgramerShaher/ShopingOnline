using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopingOnline.Data;
using ShopingOnline.Models;

namespace ShopingOnline.Controllers
{

    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly DbShopingOnlineContext _db;
        private readonly UserManager<User>? _userManager;

        public HomeController(ILogger<HomeController> logger, DbShopingOnlineContext db, UserManager<User> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Product(int id)
        {

            TempData["CategoryId"] = id;
            var result = _db.Products.Where(x => x.CategoryId == id).ToList();
            var userId = HttpContext.Session.GetString("UserId");
            var cartitemscount = _db.CartItems.Where(x => x.UserId == userId).Sum(x => x.Quantity);
            ViewBag.CartItemsCount = cartitemscount;

            return View(result);
        }

        public IActionResult Categories()
        {

            return View(_db.Categories.ToList());

        }
        [HttpGet]
        [Authorize]

        public IActionResult AddToCart(int id)
        {

            var product = _db.Products.FirstOrDefault(p => p.Id == id);
            var userid = HttpContext.Session.GetString("UserId");
            var username = HttpContext.Session.GetString("UserName");

            if (product == null)
            {
                return NotFound();
            }

            var Exectingproduct = _db.CartItems.FirstOrDefault(ci => ci.ProductId == id && ci.UserId == userid && ci.User.UserName == username);
            if (Exectingproduct != null)
            {
                Exectingproduct.Quantity++;
            }
            else
            {
                var cartitem = new CartItem
                {
                    UserId = userid,
                    ProductId = id,
                    Quantity = 1
                };
                _db.CartItems.Add(cartitem);

            }
            int categoryid = Convert.ToInt32(TempData["CategoryId"]);
            TempData["CategoryId"] = categoryid;
            _db.SaveChanges();

            return RedirectToAction("Product", new { id = categoryid });









            //var product = _db.Products.FirstOrDefault(p => p.Id == id);

            //if (product == null)
            //    return NotFound();

            //var username = HttpContext.Session.GetString("UserName");
            //var userId = HttpContext.Session.GetString("UserId");


            //var existingCartItem = _db.CartItems.FirstOrDefault(ci => ci.ProductId == id && ci.User.UserName==username && ci.UserId == userId);

            //if (existingCartItem != null)
            //{
            //    existingCartItem.Quantity++;
            //}
            //else
            //{
            //    var cartItem = new CartItem
            //    {
            //        UserId = userId,
            //        ProductId = id,
            //        Quantity = 1
            //    };
            //    _db.CartItems.Add(cartItem);
            //}

            //_db.SaveChanges();

            //// «” —Ã⁄ categoryId „‰ TempData
            //int categoryId = Convert.ToInt32(TempData["CategoryId"]);

            //// √⁄œ  Œ“Ì‰Â ·√‰ TempData Ìı” Â·ﬂ „—… Ê«Õœ…
            //TempData["CategoryId"] = categoryId;

            //// —ÃÊ⁄ ·‰›” ’›Õ… «·„‰ Ã«  Õ”» «·’‰›
            //return RedirectToAction("Product" , new {id = categoryId});
        }
        public IActionResult HotalProduct()
        {
            var result = _db.Products.Include(p => p.Category).ToList(); // <<< ÷—Ê—ÌToList();

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
                _db.Contacts.Add(model);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View("Contact");
        }
        // Ì⁄—÷ Ã„Ì⁄ «·—”«∆· «· Ì Ì—”·Â« «·„” Œœ„Ì‰ «Ê «·⁄„·«¡ 
        public IActionResult Messages()
        {

            var result = _db.Contacts.ToList();
            if (result == null)
                return NotFound();

            return View(result);
        }
        // ⁄—÷  ›«’Ì· «·„‰ Ã „⁄ «· ⁄·Ìﬁ« 
        public IActionResult ProductDetails(int id)
        {
            var product = _db.Products
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
        public async Task<IActionResult> AddComment(ProductCommentsViewModel model)
        {
            if (model.NewCommentText != null)
            {
                var username = HttpContext.Session.GetString("UserName");
                var userid = HttpContext.Session.GetString("UserId");

                if (userid == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var newComment = new Comment
                {
                    UserId = userid,
                    ProductId = model.Product.Id,  // <--  „ «·«⁄ „«œ ⁄·Ï «·ﬁÌ„… «·„—”·… „‰ «·›Ê—„
                    CommentText = model.NewCommentText,
                    Stars = model.NewStars,
                    CreatedAt = DateTime.Now
                };

                _db.Comments.Add(newComment);
                await _db.SaveChangesAsync();

                return RedirectToAction("ProductDetails", new { id = model.Product.Id });
            }

            var product = _db.Products
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.Id == model.Product.Id);

            model.Product = product;
            model.Comments = product?.Comments.ToList() ?? new List<Comment>();

            return View("ProductDetails", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
