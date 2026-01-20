using System;
using System.Web.Mvc;

namespace WebBanNuoc.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password, bool? rememberMe)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nh?p email và m?t kh?u.";
                return View();
            }

            // Demo login (hardcoded). Thay b?ng logic xác th?c th?c t? sau này.
            if (email.Equals("user@sipandsavor.vn", StringComparison.OrdinalIgnoreCase) && password == "123456")
            {
                TempData["LoginSuccess"] = "??ng nh?p thành công.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email ho?c m?t kh?u không ?úng.";
            return View();
        }

        // --- ??NG KÝ (REGISTER)

        // 1. Hi?n th? form ??ng ký
        [HttpGet]
        [AllowAnonymous] // <-- Quan tr?ng: Cho phép khách vãng lai truy c?p
        public ActionResult Register()
        {
            // L?nh này s? tìm file Views/Account/Register.cshtml ?? hi?n th?
            return View();
        }

        // 2. X? lý khi b?m nút ??ng ký
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string FullName, string Email, string PhoneNumber, string Password, string ConfirmPassword)
        {
            // Ki?m tra d? li?u
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "Vui lòng ?i?n ??y ?? thông tin.";
                return View();
            }

            if (Password != ConfirmPassword)
            {
                ViewBag.Error = "M?t kh?u xác nh?n không kh?p.";
                return View();
            }

            // TODO: Code l?u user vào Database t?i ?ây
            // ...

            // Gi? l?p thành công -> Chuy?n sang trang ??ng nh?p
            TempData["SuccessMessage"] = "??ng ký thành công! Vui lòng ??ng nh?p.";
            return RedirectToAction("Login");
        }

        // GET: /Account/Profile (placeholder)
        [HttpGet]
        public ActionResult Profile()
        {
            TempData["Info"] = "Ch?c n?ng h? s? s? ???c c?p nh?t sau.";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Orders (placeholder)
        [HttpGet]
        public ActionResult Orders()
        {
            TempData["Info"] = "Ch?c n?ng ??n hàng s? ???c c?p nh?t sau.";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Logout
        [HttpGet]
        public ActionResult Logout()
        {
            TempData["Info"] = "B?n ?ã ??ng xu?t.";
            return RedirectToAction("Index", "Home");
        }
    }
}
