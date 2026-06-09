using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyEShop.Data;
using MyEShop.Models;

namespace MyEShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyShopContext _context;
        private static Cart _cart=new Cart();

        public HomeController(ILogger<HomeController> logger,MyShopContext context) 
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {

            //var categorieswithproducts = _context.Categories
            //    .Select(c => new CategoryWithProducts
            //    {
            //        Category = c,
            //        Products = c.CategoryToProducts.Select(c=>c.Product).ToList()




            //    }).ToList();
            ShopViewModel model = new ShopViewModel()
            {

                Products = _context.Products.Include(p => p.Item).ToList()
                //CategoriesWithProducts = categorieswithproducts

            };
            return View(model);
        }

        public  IActionResult Detail(int id)
     {
         var product = _context.Products.Include(c => c.Item).SingleOrDefault(p => p.Id == id);

         var showCategory = _context.Products
             .Where(p => p.Id == id)
             .SelectMany(c => c.CategoryToProducts)
             .Select(s => s.Category)
             .ToList();

         var categories = _context.CategoryToProducts
             .Where(cp => cp.ProductId == id)
             .Include(cp => cp.Category)
             .Select(cp => cp.Category)
             .ToList();

        
         var relatedProductIds = _context.CategoryToProducts
             .Where(cp => categories.Select(c => c.Id).Contains(cp.CategoryId))
             .Select(cp => cp.ProductId)
             .Distinct()
             .Where(pid => pid != id)
             .ToList();

         var relatedProducts = _context.Products
             .Where(p => relatedProductIds.Contains(p.Id))
             .ToList();

            if (product==null)
            {
                return NotFound();
            }
            else
            {
                var model = new DetailsViewMode()
                {
                    Product = product,
                    Category = showCategory,
                    Products = relatedProducts

                };


                return View(model);
            }
         

        }

        public IActionResult AddToCart(int itemId)
        {
            var product = _context.Products.Include(p => p.Item).SingleOrDefault(p => p.ItemId == itemId);
            if (product!=null)
            {
                var cartitem = new CartItem()
                {
                    Item = product.Item,
                    Quantity = 1

                };
                _cart.AddItem(cartitem);


            }

            return RedirectToAction("ShowCartItem");

        }
      
        public IActionResult ShowCartItem()
        {
            var cartVM = new CartViewModel()
            {
                CartItems = _cart.CartItems,
                OrderTotal = _cart.CartItems.Sum(c=>c.GetTotalPrice())

            };
            return View("Cart",cartVM);


        }

        public IActionResult RemoveItem(int itemId)
        {

            _cart.RemoveItem(itemId);
            return RedirectToAction("ShowCartItem");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Route("AboutUs")]
        public IActionResult About()
        {
            return View();
        }
    }
}
