using NguyenKimHoangAnh.Context;
using Stripe;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NguyenKimHoangAnh.Controllers
{
    public class CartController : Controller
    {
        private ASPEntities _context = new ASPEntities();

        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session["Cart"] as List<Cart> ?? new List<Cart>();
            var total = cart.Sum(c => c.TotalPrice);

            // Kiểm tra giá trị của session
            if (cart != null)
            {
                ViewBag.Total = total.ToString("C", new CultureInfo("vi-VN"));
                ViewBag.CartItemCount = cart.Count;
            }
            else
            {
                ViewBag.Total = "0";
                ViewBag.CartItemCount = 0;
            }

            return View(cart);
        }


        // GET: ProductList
        public ActionResult ProductList()
        {
            var products = _context.Products.ToList(); // Lấy danh sách sản phẩm từ cơ sở dữ liệu
            return View(products); // Trả về view với danh sách sản phẩm
        }

        [HttpPost]
        public JsonResult AddToCart(int productId, int quantity = 1)
        {
            if (quantity <= 0)
            {
                return Json(new { Message = "Số lượng sản phẩm không hợp lệ!" });
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return Json(new { Message = "Sản phẩm không tồn tại!" });
            }

            var cart = Session["Cart"] as List<Cart> ?? new List<Cart>();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.TotalPrice = existingItem.Quantity * existingItem.Price;
            }
            else
            {
                var newItem = new Cart
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    ProductImage = product.ImageUrl, // Đây là nơi bạn gán ảnh
                    Quantity = quantity,
                    Price = product.Price,
                    TotalPrice = product.Price * quantity
                };
                cart.Add(newItem);
                // Kiểm tra giá trị ProductImage
                Console.WriteLine($"ProductImage: {newItem.ProductImage}");
            }

            Session["Cart"] = cart;

            var total = cart.Sum(c => c.TotalPrice);

            return Json(new
            {
                Message = "Sản phẩm đã được thêm vào giỏ hàng!",
                Count = cart.Sum(c => c.Quantity), // Số lượng sản phẩm
                Total = total.ToString("C", new CultureInfo("vi-VN"))
            });
        }
        [HttpPost]
        public JsonResult Update(int productId, int quantity)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
            }

            var cart = Session["Cart"] as List<Cart> ?? new List<Cart>();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
                existingItem.Price = product.Price; // Cập nhật giá từ cơ sở dữ liệu nếu cần
                existingItem.TotalPrice = existingItem.Price * quantity;
                Session["Cart"] = cart;

                var newTotalPrice = cart.Sum(c => c.TotalPrice);

                return Json(new
                {
                    success = true,
                    newPrice = existingItem.TotalPrice.ToString("C", new CultureInfo("vi-VN")),
                    newTotalPrice = newTotalPrice.ToString("C", new CultureInfo("vi-VN"))
                });
            }

            return Json(new { success = false, message = "Sản phẩm không có trong giỏ hàng!" });
        }


        [HttpPost]
        public JsonResult Remove(int id)
        {
            var cart = Session["Cart"] as List<Cart> ?? new List<Cart>();

            var itemToRemove = cart.FirstOrDefault(c => c.ProductId == id);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove); // Xóa sản phẩm khỏi giỏ hàng
                Session["Cart"] = cart;
                return Json(new { Message = "Sản phẩm đã được xóa khỏi giỏ hàng", Count = cart.Count });
            }

            return Json(new { Message = "Sản phẩm không tồn tại trong giỏ hàng" });
        }

        private const string stripeSecretKey = "pk_test_4eC39HqLyjWDarjtT1zdp7dc"; // Thay thế bằng khóa bí mật thực tế của bạn

        public ActionResult Payment()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PayWithCreditCard(string stripeToken)
        {
            StripeConfiguration.ApiKey = stripeSecretKey;

            var options = new ChargeCreateOptions
            {
                Amount = 5000, // Amount in cents
                Currency = "usd",
                Description = "Example charge",
                Source = stripeToken,
            };
            var service = new ChargeService();
            Charge charge = await service.CreateAsync(options);

            if (charge.Status == "succeeded")
            {
                return RedirectToAction("PaymentReturn", new { message = "Payment succeeded!" });
            }

            return RedirectToAction("PaymentReturn", new { message = "Payment failed!" });
        }

        [HttpPost]
        public ActionResult PayWithVNPay(string orderID)
        {
            // Your VNPay integration logic here
            // Redirect to VNPay payment URL
            return RedirectToAction("PaymentReturn", new { message = "Processing VNPay payment!" });
        }

        public ActionResult PaymentReturn(string message)
        {
            ViewBag.Message = message;
            return View();
        }


        private decimal GetCartTotal(string currency)
        {
            var cart = Session["Cart"] as List<Cart> ?? new List<Cart>();
            if (currency == "USD")
            {
                // Giả sử giá trị USD
                return cart.Sum(c => c.TotalPrice / 23000); // Chuyển đổi VND sang USD với tỷ giá giả sử là 1 USD = 23000 VND
            }
            else if (currency == "VND")
            {
                // Giá trị VND
                return cart.Sum(c => c.TotalPrice);
            }
            return 0;
        }


    }

}