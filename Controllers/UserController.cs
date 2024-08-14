using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using NguyenKimHoangAnh.Context;

namespace NguyenKimHoangAnh.Controllers
{
    public class UserController : Controller
    {
        private ASPEntities1 _context;

        public UserController()
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
                Session["isLoggedIn"] = true; // Đặt session isLoggedIn là true khi đăng nhập thành công
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Đăng nhập không thành công
                ViewBag.Message = "Email hoặc mật khẩu không đúng. Vui lòng kiểm tra lại.";
                return View(objUser);
            }
        }

        // GET: User/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User objUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Hash the password
                    objUser.Password = CreateMD5(objUser.Password);

                    // Add user to the database
                    _context.Users.Add(objUser);
                    _context.SaveChanges();

                    // Set success message
                    TempData["Message"] = "Registration successful! Please login.";
                    return RedirectToAction("Login", "User");
                }
                else
                {
                    // Model is invalid, show error messages
                    ViewBag.Message = "Please correct the errors and try again.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "An error occurred: " + ex.Message;
            }

            // Return the same model with error messages
            return View(objUser);
        }
        public ActionResult Logout()
        {
            Session["username"] = null;
            Session["isLoggedIn"] = null; // Hoặc có thể đặt lại thành false
            return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang Home sau khi đăng xuất
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
