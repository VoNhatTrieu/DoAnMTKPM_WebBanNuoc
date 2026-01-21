using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanNuoc.Models.Entities;
using WebBanNuoc.Models.Strategies;
using WebBanNuoc.Services;
using WebBanNuoc.Services.Implementations;

namespace WebBanNuoc.Controllers
{
    /// <summary>
    /// Home Controller
    /// Sử dụng Local Services với dữ liệu tĩnh
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public HomeController()
        {
            var pricingStrategy = new StandardPricingStrategy();
            _productService = new LocalProductService(pricingStrategy);
            _orderService = new LocalOrderService();
        }

        public ActionResult Index()
        {
            // Lấy sản phẩm mới và bán chạy để hiển thị
            ViewBag.NewArrivals = _productService.GetNewArrivals(6);
            ViewBag.TopSelling = _productService.GetTopSellingProducts(6);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

       

        // API endpoint để tìm kiếm sản phẩm
        [HttpGet]
        public JsonResult SearchProducts(string keyword)
        {
            var products = _productService.SearchProducts(keyword);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        // API endpoint để lấy chi tiết sản phẩm
        [HttpGet]
        public JsonResult GetProduct(int id)
        {
            var product = _productService.GetProductById(id);
            return Json(product, JsonRequestBehavior.AllowGet);
        }
            public ActionResult Delivery()
        {
            return View();
        }

        public ActionResult Return()
        {
            return View();
        }

        public ActionResult Guide()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

    }
}

