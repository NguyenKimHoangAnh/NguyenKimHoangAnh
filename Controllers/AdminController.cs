using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using NguyenKimHoangAnh.Context;

namespace NguyenKimHoangAnh.Controllers
{
    public class AdminController : Controller
    {
        private ASPEntities1 _context;
        public ActionResult Index()
        {
            return View();
        }


        public AdminController()
        {
            _context = new ASPEntities1();
        }

        // GET: User/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User objUser)
        {
            objUser.Password = CreateMD5(objUser.Password);
            var user = _context.Users
                .Where(u => u.Email == objUser.Email && u.Password == objUser.Password)
                .FirstOrDefault();

            if (user != null)
            {
                // Đăng nhập thành công
                Session["username"] = user.Email;
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                // Đăng nhập không thành công
                ViewBag.Message = "Email hoặc mật khẩu không đúng. Vui lòng kiểm tra lại.";
                return View(objUser);
            }
        }
        [HttpGet]
        public ActionResult Logout()
        {
            // Xóa session
            Session.Clear();

            // Đăng xuất người dùng
            FormsAuthentication.SignOut();

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Index", "Admin");
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
