using System;
using System.ComponentModel;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopingOnline.Models;
using ShopingOnline.ViewModels;

public class CartController : Controller
{
    private readonly DbShopingOnlineContext db;
    private readonly IConverter _converter;

    public CartController(DbShopingOnlineContext context , IConverter converter )
    {

        db = context;
        _converter = converter;
    }

    public IActionResult Index()
    {
        var username = HttpContext.Session.GetString("UserName");
        var userId = HttpContext.Session.GetString("UserId");
         
        var cartItems = db.CartItems
            .Where(ci => ci.UserId == userId && ci.User.UserName == username)
            .Include(ci => ci.Product)
            .ToList();

        return View(cartItems);
    }
    [HttpPost]
    public IActionResult UpdateQuantity(int cartItemId, int quantity)
    {
        var cartItem = db.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);

        if (cartItem == null)
            return NotFound();

        // الحد الأدنى 1
        cartItem.Quantity = quantity < 1 ? 1 : quantity;
        db.SaveChanges();

        return RedirectToAction("Index");
    }
    public IActionResult Remove(int id)
    {
        var cartItem = db.CartItems.FirstOrDefault(ci => ci.Id == id);
        if (cartItem == null)
        {
            return NotFound();
        }

        db.CartItems.Remove(cartItem);
        db.SaveChanges();

        return RedirectToAction("Index");
    }
    
    public IActionResult ClearCart()
    {
        var username = HttpContext.Session.GetString("UserName");
        var userId = HttpContext.Session.GetString("UserId");
        var userCartItems = db.CartItems.Where(ci => ci.UserId == userId && ci.User.UserName == username).ToList();

        db.CartItems.RemoveRange(userCartItems);
        db.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult Checkout()
    {
        var username = HttpContext.Session.GetString("UserName");
        var userId = HttpContext.Session.GetString("UserId");
        var user = db.Users.FirstOrDefault(u => u.Id == userId);
        var cartItems = db.CartItems
            .Where(c => c.UserId == userId && c.User.UserName==username).Include(c => c.Product).ToList();

        var subTotal = cartItems.Sum(item => item.Product.Price * item.Quantity);
        var viewModel = new CheckoutViewModel
        {
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            CartItems = cartItems,
            SubTotal = subTotal
        };

        return View(viewModel);
    }
    [HttpGet]
    public IActionResult ConfirmOrder()
    {
        // جهّز نموذج فارغ مثلاً
       
        return View();
    }


    [HttpPost]
    [ActionName("ConfirmOrder")]
    public IActionResult ConfirmOrderPost()
    {
        var username = HttpContext.Session.GetString("UserName");
        var userId = HttpContext.Session.GetString("UserId");
        // استرجاع العناصر في السلة الخاصة بالمستخدم
        var cartItems = db.CartItems
            .Where(c => c.UserId == userId && c.User.UserName == username)
            .Include(c => c.Product)
            .ToList();

        if (!cartItems.Any())
            return RedirectToAction("Index", "Cart");  // إذا كانت السلة فارغة، عد إلى صفحة السلة.

        // إنشاء الطلب وتخزين التفاصيل
        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            TotalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity),  // حساب الإجمالي
            Status = "قيد المعالجة",  // تعيين الحالة بشكل افتراضي
            OrderDetails = cartItems.Select(c => new OrderDetail
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                Price = c.Product.Price
            }).ToList()
        };

        // إضافة الطلب إلى قاعدة البيانات
        db.Orders.Add(order);

        // حذف العناصر من السلة بعد إنشاء الطلب
        db.CartItems.RemoveRange(cartItems);
        db.SaveChanges();

        // إعادة التوجيه إلى صفحة النجاح مع رقم الطلب
        return RedirectToAction("OrderSuccess", new { orderId = order.Id });
    }

    // عرض صفحة نجاح الطلب
    public IActionResult OrderSuccess(int orderId)
    {
        // جلب تفاصيل الطلب باستخدام orderId
        var order = db.Orders.Include(o => o.OrderDetails)
                             .ThenInclude(od => od.Product)  // إضافة معلومات المنتجات
                             .FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            return NotFound();  // إذا لم يتم العثور على الطلب
        }

        return View(order);  // عرض الـView مع تفاصيل الطلب
    }
    public IActionResult Invoice(int orderId)
    {
        var order = db.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .FirstOrDefault(o => o.Id == orderId);

        if (order == null)
            return NotFound();

        return View(order);

    }


}
