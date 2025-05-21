using System;
using System.ComponentModel;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopingOnline.Data;
using ShopingOnline.Models;
using ShopingOnline.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPalCheckoutSdk.Orders;
using ShopingOnline.Services; // للكلاس PayPalClient
using System.Text.Json;
public class CartController : Controller
{
    private readonly DbShopingOnlineContext _db;
    private readonly IConverter _converter;

    public CartController(DbShopingOnlineContext db )
    {

        _db = db;
        
    }

    public IActionResult Index()
    {

        var username = HttpContext.Session.GetString("UserName");
        var userid = HttpContext.Session.GetString("UserId");
        var cartitems = _db.CartItems.Where(ci => ci.UserId == userid && ci.User.UserName == username)
            .Include(ci => ci.Product).ToList();
        return View(cartitems);











        //var username = HttpContext.Session.GetString("UserName");
        //var userId = HttpContext.Session.GetString("UserId");
         
        //var cartItems = _db.CartItems
        //    .Where(ci => ci.UserId == userId && ci.User.UserName == username)
        //    .Include(ci => ci.Product)
        //    .ToList();

        //return View(cartItems);
    }
    [HttpPost]
    public IActionResult UpdateQuantity(int cartItemId, int quantity)
    {
        var productid = _db.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
        if (productid == null)

            return NotFound();

        else

            productid.Quantity = quantity < 1 ? 1 : quantity;
        _db.SaveChanges();
        return RedirectToAction("Index");
        
        //var cartItem = _db.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);

        //if (cartItem == null)
        //    return NotFound();

        //// الحد الأدنى 1
        //cartItem.Quantity = quantity < 1 ? 1 : quantity;
        //_db.SaveChanges();

        //return RedirectToAction("Index");
    }
    public IActionResult Remove(int id)
    {
        var deleteproduct = _db.CartItems.FirstOrDefault(x => x.Id == id);
        try
        {
            
            if (deleteproduct == null)

                return NotFound();

            _db.CartItems.Remove(deleteproduct);
           

        }
        catch
        {
            
        }
        return RedirectToAction("Index");









        //var cartItem = _db.CartItems.FirstOrDefault(ci => ci.Id == id);
        //if (cartItem == null)
        //{
        //    return NotFound();
        //}

        //_db.CartItems.Remove(cartItem);
        //_db.SaveChanges();

        //return RedirectToAction("Index");
    }

    public IActionResult ClearCart()
    {
        var userid = HttpContext.Session.GetString("UserId");
        var username = HttpContext.Session.GetString("UserName");


        var clearecart = _db.CartItems.Where(x => x.UserId == userid && x.User.UserName == username)
            .Include(x => x.Product);

        _db.CartItems.RemoveRange(clearecart);
        _db.SaveChanges();

        return RedirectToAction("Index");

            














        //    //var username = HttpContext.Session.GetString("UserName");
        //    //var userId = HttpContext.Session.GetString("UserId");
        //    //var userCartItems = _db.CartItems.Where(ci => ci.UserId == userId && ci.User.UserName == username).ToList();

        //    //_db.CartItems.RemoveRange(userCartItems);
        //    //_db.SaveChanges();

        //    //return RedirectToAction("Index");
    }

    public IActionResult Checkout()
    {
        var username = HttpContext.Session.GetString("UserName");
        var userId = HttpContext.Session.GetString("UserId");
        var user = _db.Users.FirstOrDefault(u => u.Id == userId);
        var cartItems = _db.CartItems
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
    public async Task<IActionResult> ConfirmOrderPost()
    {
        var username = HttpContext.Session.GetString("UserName");
        var userId = HttpContext.Session.GetString("UserId");

        var cartItems = _db.CartItems
            .Where(c => c.UserId == userId && c.User.UserName == username)
            .Include(c => c.Product)
            .ToList();

        if (!cartItems.Any())
            return RedirectToAction("Index", "Cart");

        var totalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity);

        var orderRequest = new OrderRequest()
        {
            CheckoutPaymentIntent = "CAPTURE",
            PurchaseUnits = new List<PurchaseUnitRequest>
        {
            new PurchaseUnitRequest
            {
                AmountWithBreakdown = new AmountWithBreakdown
                {
                    CurrencyCode = "USD", // يمكنك تغييره إلى عملتك إذا مدعومة
                    Value = totalAmount.ToString("F2")
                }
            }
        },
            ApplicationContext = new ApplicationContext
            {
                ReturnUrl = Url.Action("PayPalSuccess", "Cart", null, Request.Scheme),
                CancelUrl = Url.Action("Checkout", "Cart", null, Request.Scheme)
            }
        };

        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(orderRequest);

        var response = await PayPalClient.Client().Execute(request);
        var statusCode = response.StatusCode;
        var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

        // حفظ رقم الطلب في Session مؤقتًا لاستخدامه لاحقًا
        HttpContext.Session.SetString("PayPalOrderId", result.Id);
        HttpContext.Session.SetString("TotalAmount", totalAmount.ToString());

        // توجيه المستخدم إلى PayPal
        var approvalLink = result.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;
        return Redirect(approvalLink);
    }

    // عرض صفحة نجاح الطلب

    public async Task<IActionResult> PayPalSuccess(string token)
    {
        var userId = HttpContext.Session.GetString("UserId");
        var username = HttpContext.Session.GetString("UserName");

        var captureRequest = new OrdersCaptureRequest(token);
        captureRequest.RequestBody(new OrderActionRequest());

        var response = await PayPalClient.Client().Execute(captureRequest);
        var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

        if (result.Status == "COMPLETED")
        {
            // ✅ يتم إنشاء الفاتورة فقط إذا تمت عملية الدفع بنجاح
            var cartItems = _db.CartItems
                .Where(c => c.UserId == userId && c.User.UserName == username)
                .Include(c => c.Product)
                .ToList();

            var order = new ShopingOnline.Models.Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = decimal.Parse(HttpContext.Session.GetString("TotalAmount")),
                OrderStatusNew = OrderStatus.جديد,
                OrderDetails = cartItems.Select(c => new OrderDetail
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.Product.Price
                }).ToList()
            };

            _db.Orders.Add(order);
            _db.CartItems.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            return RedirectToAction("OrderSuccess", new { orderId = order.Id });
        }

        return RedirectToAction("Checkout", "Cart");
    }

    public IActionResult OrderSuccess(int orderId)
    {
        // جلب تفاصيل الطلب باستخدام orderId
        var order = _db.Orders.Include(o => o.OrderDetails)
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
        var order = _db.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .FirstOrDefault(o => o.Id == orderId);

        if (order == null)
            return NotFound();

        return View(order);

    }


}
