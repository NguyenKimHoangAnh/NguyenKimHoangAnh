using NguyenKimHoangAnh.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NguyenKimHoangAnh.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ASPEntities _context;

        public ProductsController()
        {
            _context = new ASPEntities(); // Khởi tạo context
        }

        public ActionResult Index()
        {
            var products = _context.Products.ToList();
            ViewBag.ItemCount = products.Count; // Đếm số lượng sản phẩm
            return View(products);
        }

        public ActionResult Create()
        {
            SetDropDownLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        // Xác định tên tệp và đường dẫn lưu trữ
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(Server.MapPath("~/images/pro"), fileName);

                        // Lưu tệp vào thư mục
                        imageFile.SaveAs(filePath);

                        // Cập nhật URL hình ảnh trong đối tượng sản phẩm
                        product.ImageUrl = fileName;
                    }

                    _context.Products.Add(product);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi
                    ModelState.AddModelError("", "Lỗi xảy ra khi lưu tệp ảnh: " + ex.Message);
                }
            }

            SetDropDownLists(product);
            return View(product);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = _context.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            SetDropDownLists(product);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = _context.Products.Find(product.Id);
                    if (existingProduct == null)
                    {
                        return HttpNotFound();
                    }

                    // Nếu có ảnh mới được tải lên
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        // Xác định tên tệp và đường dẫn lưu trữ
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(Server.MapPath("~/images/pro"), fileName);

                        // Lưu tệp vào thư mục
                        imageFile.SaveAs(filePath);

                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                        {
                            var oldFilePath = Path.Combine(Server.MapPath("~/images/pro"), existingProduct.ImageUrl);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Cập nhật URL hình ảnh trong đối tượng sản phẩm
                        existingProduct.ImageUrl = fileName;
                    }

                    // Cập nhật các thuộc tính khác
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.Description = product.Description;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.BrandId = product.BrandId;

                    _context.Entry(existingProduct).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi xảy ra khi lưu tệp ảnh: " + ex.Message);
                }
            }

            // Nếu có lỗi, chuẩn bị danh sách dropdown cho các thuộc tính liên quan
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            return View(product);
        }


        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .SingleOrDefault(p => p.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = _context.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = _context.Products.Find(id);
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private void SetDropDownLists(Product product = null)
        {
            var categories = _context.Categories.Select(c => new { Id = c.CategoryId, Name = c.Name }).ToList();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", product?.CategoryId);

            var brands = _context.Brands.AsNoTracking().Select(b => new { Id = b.BrandId, Name = b.BrandName }).ToList();
            ViewBag.BrandId = new SelectList(brands, "Id", "Name", product?.BrandId);
        }
    }
}
