using System;
using System.Web.Mvc;

namespace WebBanNuoc.Controllers
{
    /// <summary>
    /// Admin Controller - Quản lý trang quản trị
    /// </summary>
    public class AdminController : Controller
    {
        // Check if user is logged in and is Admin
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                filterContext.Result = RedirectToAction("Login", "Account");
                return;
            }
            
            // Check if user is Admin
            var userRole = Session["UserRole"]?.ToString();
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này!";
                filterContext.Result = RedirectToAction("Index", "Home");
                return;
            }
            
            // Pass user info to ViewBag for all admin pages
            ViewBag.UserName = Session["UserName"];
            ViewBag.UserEmail = Session["UserEmail"];
        }
        
        // GET: Admin Dashboard
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Products
        public ActionResult Products()
        {
            return View();
        }

        // GET: Admin/Categories
        public ActionResult Categories()
        {
            return View();
        }

        // GET: Admin/Orders
        public ActionResult Orders()
        {
            return View();
        }

        // GET: Admin/Customers
        public ActionResult Customers()
        {
            return View();
        }

        // GET: Admin/Reports
        public ActionResult Reports()
        {
            return View();
        }
        
        // GET: Admin/Test - For debugging routing and session
        public ActionResult Test()
        {
            return View();
        }
    }
}
