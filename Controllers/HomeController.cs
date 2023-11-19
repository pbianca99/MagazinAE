using MagazinAE.Models.Entities;
using MagazinAE.Models.VMs;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MagazinAE.Controllers
{
    public class HomeController : Controller
    {
        private readonly MagazinAEContext context;

        public HomeController(MagazinAEContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {

            var list = context.Products.ToList().Select(p => new ProductVM().ProdToProdVM(p)).ToList();
            return View(list);
        }

        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            var productVM = new ProductVM().ProdToProdVM(product);
            return View(productVM);
        }

        [HttpPost]
        [Route("Add/{id}")]
        public IActionResult Add(int id)
        {
            var shopList = HttpContext.Session.Get<List<int>>(SessionHelper.ShoppingCart);

            if (shopList == null)
                shopList = new List<int>();

            if (!shopList.Contains(id))
                shopList.Add(id);

            HttpContext.Session.Set(SessionHelper.ShoppingCart, shopList);

            return RedirectToAction("Index", "Home", Product.GetAll(context));
        }

        [HttpPost]
        [Route("Remove/{id}")]
        public IActionResult Remove(int id)
        {
            var shopList = HttpContext.Session.Get<List<int>>(SessionHelper.ShoppingCart);

            if (shopList == null)
                return RedirectToAction("Index", "Home", context.Products.Select(p => new ProductVM().ProdToProdVM(p)).ToList());

            if (shopList.Contains(id))
                shopList.Remove(id);

            HttpContext.Session.Set(SessionHelper.ShoppingCart, shopList);


            return RedirectToAction("Index", "Home", context.Products.Select(p => new ProductVM().ProdToProdVM(p)).ToList());
        }

    }
}