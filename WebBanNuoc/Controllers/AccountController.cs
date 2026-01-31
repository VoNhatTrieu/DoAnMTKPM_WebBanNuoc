using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace WebBanNuoc.Controllers
{
    public class AccountController : Controller
    {
        // Thử HTTP trước (không cần SSL), nếu không được thì dùng HTTPS
        private readonly string _apiBaseUrl = "http://localhost:5299/api";
        private static readonly HttpClient _httpClient = CreateHttpClient();

        private static HttpClient CreateHttpClient()
        {
            // Bypass SSL certificate validation cho development
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            
            return client;
        }

        // GET: /Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string email, string password, bool? rememberMe)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập email và mật khẩu.";
                return View();
            }

            try
            {
                // Gọi API Backend
                var loginDto = new
                {
                    email = email,
                    password = password,
                    rememberMe = rememberMe ?? false
                };

                var json = JsonConvert.SerializeObject(loginDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<LoginResponse>>(responseContent);

                if (result != null && result.Success && result.Data != null)
                {
                    // Lưu thông tin user vào Session
                    Session["UserId"] = result.Data.UserInfo.Id;
                    Session["UserName"] = result.Data.UserInfo.FullName;
                    Session["UserEmail"] = result.Data.UserInfo.Email;
                    Session["UserRole"] = result.Data.UserInfo.Role;
                    Session["Token"] = result.Data.Token;

                    TempData["SuccessMessage"] = result.Message; // "Đăng nhập thành công"
                    
                    // Redirect based on user role
                    if (result.Data.UserInfo.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Error = result?.Message ?? "Đăng nhập thất bại. Vui lòng thử lại.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi kết nối đến server: {ex.Message}";
                return View();
            }
        }

        // --- ĐĂNG KÝ (REGISTER)

        // 1. Hiển thị form đăng ký
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // 2. Xử lý khi bấm nút đăng ký
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(string FullName, string Email, string PhoneNumber, string Password, string ConfirmPassword)
        {
            // Kiểm tra dữ liệu
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }

            if (Password != ConfirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            try
            {
                // Gọi API Backend
                var registerDto = new
                {
                    fullName = FullName,
                    email = Email,
                    phoneNumber = PhoneNumber,
                    password = Password,
                    confirmPassword = ConfirmPassword
                };

                var json = JsonConvert.SerializeObject(registerDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Auth/register", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<string>>(responseContent);

                if (result != null && result.Success)
                {
                    TempData["SuccessMessage"] = result.Message; // "Đăng ký thành công! Vui lòng đăng nhập."
                    return RedirectToAction("Login");
                }
                else
                {
                    // Hiển thị lỗi validation từ API
                    if (result?.Errors != null && result.Errors.Count > 0)
                    {
                        ViewBag.Error = string.Join("<br/>", result.Errors);
                    }
                    else
                    {
                        ViewBag.Error = result?.Message ?? "Đăng ký thất bại. Vui lòng thử lại.";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi kết nối đến server: {ex.Message}";
                return View();
            }
        }

        // GET: /Account/Profile (placeholder)
        [HttpGet]
        public ActionResult Profile()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.UserName = Session["UserName"];
            ViewBag.UserEmail = Session["UserEmail"];
            ViewBag.PhoneNumber = Session["PhoneNumber"];
            return View();
        }

        // GET: /Account/Orders (Lịch sử đơn hàng)
        [HttpGet]
        public async Task<ActionResult> Orders()
        {
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem lịch sử đơn hàng.";
                return RedirectToAction("Login");
            }

            try
            {
                int userId = (int)Session["UserId"];
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Orders/user/{userId}");
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<System.Collections.Generic.List<OrderDto>>>(responseContent);

                if (result != null && result.Success && result.Data != null)
                {
                    ViewBag.Orders = result.Data;
                }
                else
                {
                    ViewBag.Orders = new System.Collections.Generic.List<OrderDto>();
                    ViewBag.Message = "Bạn chưa có đơn hàng nào.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Orders = new System.Collections.Generic.List<OrderDto>();
                ViewBag.Error = $"Lỗi tải đơn hàng: {ex.Message}";
            }

            return View();
        }

        // GET: /Account/Logout
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            // Xóa cookies xác thực nếu có
            if (Request.Cookies["auth_token"] != null)
            {
                var cookie = new System.Web.HttpCookie("auth_token")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Value = ""
                };
                Response.Cookies.Add(cookie);
            }
            TempData["SuccessMessage"] = "Bạn đã đăng xuất.";
            // Set flag to clear localStorage on client side
            TempData["ClearCart"] = true;
            return RedirectToAction("Index", "Home");
        }

        // Helper classes để deserialize API response
        private class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
            public System.Collections.Generic.List<string> Errors { get; set; }
        }

        private class LoginResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string Token { get; set; }
            public UserInfo UserInfo { get; set; }
        }

        private class UserInfo
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Role { get; set; }
        }

        private class OrderDto
        {
            public int Id { get; set; }
            public string OrderNumber { get; set; }
            public DateTime OrderDate { get; set; }
            public string Status { get; set; }
            public string PaymentMethod { get; set; }
            public string CustomerName { get; set; }
            public string CustomerPhone { get; set; }
            public string ShippingAddress { get; set; }
            public decimal Total { get; set; }
        }
    }
}
