using NguyenKimHoangAnh.Context;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NguyenKimHoangAnh.Areas.Admin.Controllers
{
    public class BrandsController : Controller
    {
        private readonly ASPEntities _context;

        public BrandsController()
        {
            _context = new ASPEntities(); // Khởi tạo context
        }

        // GET: Admin/Brands
        public ActionResult Index()
        {
            var brands = _context.Brands.ToList();
            return View(brands);
        }

        // GET: Admin/Brands/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Brand brand, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Xác định tên tệp và đường dẫn lưu trữ
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(Server.MapPath("~/images/brands"), fileName);

                    // Lưu tệp vào thư mục
                    imageFile.SaveAs(filePath);

                    // Cập nhật URL hình ảnh trong đối tượng nhãn hiệu
                    brand.ImageUrl = fileName;
                }

                _context.Brands.Add(brand);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(brand);
        }


        // GET: Admin/Brands/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Brand brand = _context.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }

            return View(brand);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Brand brand, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm đối tượng Brand hiện tại trong cơ sở dữ liệu
                    var brandInDb = _context.Brands.Find(brand.BrandId);
                    if (brandInDb == null)
                    {
                        return HttpNotFound();
                    }

                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        // Xác định tên tệp và đường dẫn lưu trữ
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(Server.MapPath("~/images/brands"), fileName);

                        // Kiểm tra và tạo thư mục nếu không tồn tại
                        var directory = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        // Lưu tệp vào thư mục
                        imageFile.SaveAs(filePath);

                        // Cập nhật URL hình ảnh trong đối tượng Brand
                        brandInDb.ImageUrl = fileName;
                    }
                    else
                    {
                        // Nếu không có ảnh mới, giữ lại ảnh cũ
                        brandInDb.ImageUrl = brandInDb.ImageUrl;
                    }

                    // Cập nhật các thuộc tính khác
                    brandInDb.BrandName = brand.BrandName;

                    // Đánh dấu đối tượng là đã thay đổi
                    _context.Entry(brandInDb).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi
                    ModelState.AddModelError("", "Lỗi xảy ra khi lưu tệp ảnh: " + ex.Message);
                }
            }

            return View(brand);
        }



        // GET: Admin/Brands/Detail/5
        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Brand brand = _context.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }

            var products = _context.Products.Where(p => p.BrandId == id).ToList();
            ViewBag.Products = products;

            return View(brand);
        }

        // POST: Admin/Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Brand brand = _context.Brands.Find(id);
            if (brand != null)
            {
                // Xóa tất cả sản phẩm liên quan (nếu cần)
                var products = _context.Products.Where(p => p.BrandId == id).ToList();
                _context.Products.RemoveRange(products);

                _context.Brands.Remove(brand);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
