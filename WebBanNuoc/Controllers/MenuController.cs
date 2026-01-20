using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using WebBanNuoc.Models.Entities;

namespace WebBanNuoc.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu
        [ChildActionOnly]
        public ActionResult Index(string category)
        {
            ViewBag.Category = category;
            return View();
        }

        public ActionResult CategoryMenu()
        {
            var categories = new List<Category>
            {
                new Category {Id = 1, Name = "Trà sữa ", Slug = "tra-sua" },
                new Category {Id = 2, Name = "Cà phê", Slug = "ca-phe" },
                new Category {Id = 3, Name = "Nước ép", Slug = "nuoc-ep" },
                new Category { Id = 4, Name = "Sinh tố", Slug = "sinh-to" }
            };
            return PartialView("_CategoryMenu", categories);
        }
    }
}