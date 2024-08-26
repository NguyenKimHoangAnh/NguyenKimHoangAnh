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
    public class CategoryController : Controller
    {
        private readonly ASPEntities _context;

        public CategoryController()
        {
            _context = new ASPEntities(); // Khởi tạo context
        }

        // GET: Admin/Categories
        public ActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        // GET: Admin/Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // Ví dụ về việc thêm danh mục mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(Server.MapPath("~/images/cate"), fileName);
                    imageFile.SaveAs(filePath);
                    category.ImageUrl = fileName;
                }
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }


        // GET: Admin/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = _context.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm đối tượng Category hiện tại trong cơ sở dữ liệu
                    var categoryInDb = _context.Categories.Find(category.CategoryId);
                    if (categoryInDb == null)
                    {
                        return HttpNotFound();
                    }

                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        // Xác định tên tệp và đường dẫn lưu trữ
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(Server.MapPath("~/images/cate"), fileName);

                        // Kiểm tra và tạo thư mục nếu không tồn tại
                        var directory = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        // Lưu tệp vào thư mục
                        imageFile.SaveAs(filePath);

                        // Cập nhật URL hình ảnh trong đối tượng danh mục
                        categoryInDb.ImageUrl = fileName;
                    }

                    // Cập nhật các thuộc tính khác
                    categoryInDb.Name = category.Name;

                    // Đánh dấu đối tượng là đã thay đổi
                    _context.Entry(categoryInDb).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi
                    ModelState.AddModelError("", "Lỗi xảy ra khi lưu tệp ảnh: " + ex.Message);
                }
            }

            return View(category);
        }


        // GET: Admin/Categories/Details/5
        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = _context.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = _context.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = _context.Categories.Find(id);
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
