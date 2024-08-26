using NguyenKimHoangAnh.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenKimHoangAnh.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        private ASPEntities _context = new ASPEntities();

        // GET: Admin/Users
        public ActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        // GET: Admin/Users/Details/5
        public ActionResult Details(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Admin/Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/images/user/"), fileName);
                    imageFile.SaveAs(path);
                    user.ImageUrl = fileName;
                }

                // Add new user to database
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.Find(user.Id);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật các thuộc tính của user
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;

                // Cập nhật mật khẩu nếu được cung cấp
                if (!string.IsNullOrEmpty(user.Password))
                {
                    existingUser.Password = user.Password; // Bạn có thể muốn mã hóa mật khẩu
                }

                // Xử lý upload hình ảnh
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/images/user/"), fileName);
                    imageFile.SaveAs(path);
                    existingUser.ImageUrl = fileName;
                }

                // Cập nhật quyền admin
                existingUser.IsAdmin = user.IsAdmin;

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }


        // GET: Admin/Users/Delete/5
        public ActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}