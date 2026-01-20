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
    /// Sử dụng Local Service với dữ liệu tĩnh
    /// </summary>
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController()
        {
            var pricingStrategy = new StandardPricingStrategy();
            _productService = new LocalProductService(pricingStrategy);
        }

        // GET: Products
        public ActionResult Index(string category = "", string sort = "")
        {
            IEnumerable<Models.DTOs.ProductDTO> products;

            if (!string.IsNullOrEmpty(category))
            {
                products = _productService.GetProductsByCategory(category);
            }
            else
            {
                products = _productService.GetAvailableProducts();
            }

            ViewBag.Category = category;
            ViewBag.Sort = sort;
            ViewBag.Products = products;
            return View();
        }

        // Trang chi tiết sản phẩm
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
            var products = _productService.SearchProducts(keyword);
            ViewBag.Keyword = keyword;
            ViewBag.Category = category;
            ViewBag.Sort = sort;
            ViewBag.Products = products;
            return View("Index");
        }
    }
}