using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyEShop.Data;
using MyEShop.Data.Repository;
using MyEShop.Models;

namespace MyEShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAccountRepository _accountRepository;
        private MyShopContext _context;
       

        public HomeController(ILogger<HomeController> logger,MyShopContext context, IAccountRepository accountRepository) 
        {
            _context = context;
            _accountRepository = accountRepository;
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


        [Authorize]
        public IActionResult AddToCart(int ProductId)
        {
            var product = _context.Products.Include(p => p.Item).SingleOrDefault(p => p.ItemId ==ProductId );
            if (product!=null)
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                var order = _context.Orders.FirstOrDefault(c => c.UserId == userId&&!c.IsFinaly);
                if (order!=null)
                {
                    var orderDetail = _context.OrderDetails.FirstOrDefault(d=>d.OrderId==order.OrderId
                                                                              &&d.ProductId==product.Id);
                    if (orderDetail!=null)
                    {
                        orderDetail.Count += 1;
                    }
                    else
                    {
                        _context.OrderDetails.Add(new OrderDetail()
                        {
                            OrderId = order.OrderId
                            ,
                            ProductId = product.Id
                            ,
                            Price = product.Item.Price
                            ,
                            Count = 1

                        });
                    }

                }
                else
                {
                    order=new Order()
                    {
                        UserId = userId,
                        CreationDate = DateTime.Now
                        ,IsFinaly = false

                    };
                    _context.Orders.Add(order);
                    _context.SaveChanges();
                    _context.OrderDetails.Add(new OrderDetail()
                    {
                        OrderId = order.OrderId
                        ,ProductId = product.Id
                        ,Price = product.Item.Price
                        ,Count = 1

                    });
                   



                }
                _context.SaveChanges();
            }

            return RedirectToAction("ShowCartItem");

        }
        //[Authorize]
        public IActionResult ShowCartItem()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());

            var order = _context.Orders.Where(c => c.UserId == userId&&!c.IsFinaly)
                .Include(x => x.OrderDetails)
                .ThenInclude(f => f.Product).FirstOrDefault();
            return View("Cart",order);


        }

        public IActionResult RemoveItem(int DetailId)
        {
            var orderDetail = _context.OrderDetails.Find(DetailId);
            _context.Remove(orderDetail);
          
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
        // این متد در HomeController می‌ماند
        [Authorize]
        public IActionResult SelectAddress(int addressId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
            var order = _context.Orders.FirstOrDefault(c => c.UserId == userId && !c.IsFinaly);
            if (order != null)
            {
                _accountRepository.SetOrderAddress(order.OrderId, addressId);
                // هدایت به مرحلهٔ بعد (مثلاً تأیید نهایی سفارش)
                return RedirectToAction("OrderConfirmation");
            }
            TempData["ErrorMessage"] = "سفارش فعالی یافت نشد.";
            return RedirectToAction("ShowCartItem");
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}
