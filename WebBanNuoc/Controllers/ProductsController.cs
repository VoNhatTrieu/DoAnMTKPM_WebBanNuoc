using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanNuoc.Services;
using WebBanNuoc.Services.Implementations;
using WebBanNuoc.Models.Strategies;

namespace WebBanNuoc.Controllers
{
    /// <summary>
    /// Products Controller
    /// View sử dụng JavaScript API để load dữ liệu từ apii backend
    /// </summary>
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController()
        {
            var pricingStrategy = new StandardPricingStrategy();
            _productService = new LocalProductService(pricingStrategy);
        }

        // GET: Products - View sẽ load data từ API qua JavaScript
        public ActionResult Index(string category = "", string sort = "")
        {
            ViewBag.Category = category;
            ViewBag.Sort = sort;
            return View();
        }

        // Trang chi tiết sản phẩm - Vẫn dùng LocalProductService cho đến khi migrate sang API
        public ActionResult ProductDetail(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.Product = product;
            ViewBag.ProductId = id;
            return View();
        }

        public ActionResult Search(string category, string sort, string keyword)
        {
            ViewBag.Keyword = keyword;
            ViewBag.Category = category;
            ViewBag.Sort = sort;
            return View("Index");
        }
    }
}