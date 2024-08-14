using NguyenKimHoangAnh.Context;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace NguyenKimHoangAnh.Controllers
{
    public class CartController : Controller
    {
        private ASPEntities1 _context = new ASPEntities1();

        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session["Cart"] as List<Cart> ?? new List<Cart>();
            var total = cart.Sum(c => c.TotalPrice);

            ViewBag.Total = total.ToString("C", new CultureInfo("vi-VN"));

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
                    ProductImage = product.ImageUrl,
                    Quantity = quantity,
                    Price = product.Price,
                    TotalPrice = product.Price * quantity
                };
                cart.Add(newItem);
            }

            Session["Cart"] = cart;

            var total = cart.Sum(c => c.TotalPrice);

            return Json(new
            {
                Message = "Sản phẩm đã được thêm vào giỏ hàng!",
                Count = cart.Count,
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
    }
}
