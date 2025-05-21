using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using ShopingOnline.Data;
using ShopingOnline.Models;

namespace ShopingOnline.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]


    public class DefaultController : Controller
    {
        private readonly DbShopingOnlineContext _db;

        public DefaultController(DbShopingOnlineContext db)
        {
            _db = db;
        }
        [Authorize]
        public IActionResult Index()

        {
            
            var username = HttpContext.Session.GetString("UserName");
            var fullname = HttpContext.Session.GetString("FullName");

            var userId = HttpContext.Session.GetString("UserId");
            ViewBag.Admin = HttpContext.Session.GetString("FullName");
            DateTime weekAgo = DateTime.Now.AddDays(-7);

            // حساب عدد الطلبات الجديدة
            var newOrdersCount = _db.Orders
                                  .Where(o => o.OrderStatusNew == OrderStatus.جديد && o.OrderDate >= weekAgo)
                                  .Count();
            var UserCounts = _db.Users.Count();
            var Categorycount = _db.Categories.Count();
            var Productcount = _db.Products.Count();
            
            ViewBag.UserCount = UserCounts;
            ViewBag.NewOrdersCount = newOrdersCount;
            ViewBag.Categoriescount = Categorycount;
            ViewBag.Productscount = Productcount;

            return View();
        }


        public async Task<IActionResult> OrdersList(DateTime? date, OrderStatus? statuse)
        {
            // جلب الاستعلام الأساسي للطلبات مع التفاصيل
            var ordersQuery = _db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Category)
                .AsQueryable(); // حتى نستطيع تطبيق الشروط لاحقاً

            // فلترة بالتاريخ إذا تم تحديده
            if (date.HasValue)
            {
                var targetDate = date.Value.Date;
                ordersQuery = ordersQuery.Where(o => o.OrderDate.Date == targetDate);
            }

            // فلترة بالحالة إذا تم تحديدها
            if (statuse.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderStatusNew == statuse.Value);
            }

            // تنفيذ الاستعلام
            var orders = await ordersQuery.ToListAsync();

            // الإحصائيات حسب النتائج بعد الفلترة
            ViewBag.TotalOrders = orders.Count;
            ViewBag.CompleteOrder = orders.Count(o => o.OrderStatusNew == OrderStatus.مكتمل);
            ViewBag.PendingOrders = orders.Count(o => o.OrderStatusNew == OrderStatus.قيد_المعالجة);
            ViewBag.CanceledOrders = orders.Count(o => o.OrderStatusNew == OrderStatus.ملغي);
            ViewBag.NewsOrders = orders.Count(o => o.OrderStatusNew == OrderStatus.جديد);

            // إعداد البيانات للعرض
            var viewModel = orders.Select(o => new OrderViewWithProduct
            {
                OrderId = o.Id,
                UserName = o.User?.UserName,
                Email = o.User?.Email,
                OrderDate = o.OrderDate,
                Status = o.OrderStatusNew,
                TotalAmount = o.TotalAmount,
                ProductNames = o.OrderDetails.Select(od => od.Product.Name).ToList(),
                image = o.OrderDetails.Select(od => od.Product.Image).ToList().ToString(),
                ProductDescriptions = o.OrderDetails.Select(od => od.Product.Description).ToList(),
                ProductCategories = o.OrderDetails.Select(od => od.Product.Category?.Name ?? "").Distinct().ToList()
            }).ToList();

            return View(viewModel);
        }


        public IActionResult DeleteOrder(int id)
        {
            var order = _db.Orders.FirstOrDefault(x => x.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            else
            {
                order.OrderStatusNew = OrderStatus.ملغي;

            }
            _db.SaveChanges();
            return RedirectToAction("OrdersList");
        }
              public IActionResult YesTheOrder(int id)
        {
            var order = _db.Orders.FirstOrDefault(x => x.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            else
            {
                order.OrderStatusNew = OrderStatus.مكتمل;

            }
            _db.SaveChanges();
            return RedirectToAction("OrdersList");

        }
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            var viewModel = new OrderDetailsViewModel
            {
                OrderId = order.Id,
                UserName = order.User?.UserName,
                OrderDate = order.OrderDate,
                Status = order.OrderStatusNew,
                Products = order.OrderDetails.Select(od => new OrderProductViewModel
                {
                    ProductName = od.Product.Name,
                    Description = od.Product.Description,
                    Image = od.Product.Image,
                    Price = od.Product.Price
                }).ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Invoice(int id)
        {
            var order = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var invoice = new InvoiceViewModel
            {
                OrderId = order.Id,
                CustomerName = order.User?.UserName,
                Email = order.User?.Email,
                OrderDate = order.OrderDate,
                Status = order.OrderStatusNew,
                TotalAmount = order.TotalAmount,
                Items = order.OrderDetails.Select(od => new InvoiceProductItem
                {
                    ProductName = od.Product.Name,
                    Category = od.Product.Category?.Name ?? "",
                    Quantity = od.Quantity,
                    Price = od.Product.Price
                }).ToList()
            };

            return View(invoice);
        }

    }
}
