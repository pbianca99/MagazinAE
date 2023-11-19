using MagazinAE.Models.VMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using MagazinAE.Models.Entities;
using MagazinAE.Models.VMs;
using System.Collections.Generic;
using System.Linq;


namespace MagazinAE.Controllers
{
    public class CartController : Controller
    {
        private readonly MagazinAEContext context;

        public CartController(MagazinAEContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var shoppingList = HttpContext.Session.Get<List<int>>(SessionHelper.ShoppingCart);

            
            if (shoppingList == null)
            {
                ViewBag.ErrorMessage = "Your shopping cart is empty.";
                ViewBag.Total = 0;
                return View(new CartViewModel()); 
            }

            var productsInCart = context.Products.Where(p => shoppingList.Contains(p.Id))
                                                .Select(p => new ProductVM().ProdToProdVM(p))
                                                .ToList();

            var total = productsInCart.Sum(p => p.Price); 
            
            ViewBag.Total = total;

            var cartViewModel = new CartViewModel
            {
                ProductsInCart = productsInCart,
                Total = total
            };

            return View(cartViewModel);
        }


        [HttpGet]
        [Route("GetCartDetails")]
        public IActionResult GetCart()
        {
            var shoppingList = HttpContext.Session.Get<List<int>>(SessionHelper.ShoppingCart);
            var productsInCart = context.Products.Where(p => shoppingList.Contains(p.Id)).ToList();

            return PartialView("_CartPartial", productsInCart);
        }

        public class CartViewModel
        {
            public List<ProductVM> ProductsInCart { get; set; }
            public decimal Total { get; set; }
        }

    }
}
