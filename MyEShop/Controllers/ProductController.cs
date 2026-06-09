using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEShop.Data;

namespace MyEShop.Controllers
{
    public class ProductController : Controller
    {
        private MyShopContext _context;
        public ProductController(MyShopContext context)
        {
            _context = context;
        }
        [Route("Group/{id}/{name}")]
        public IActionResult ShowProductByGroup(int id,string name)
        {
            ViewData["GroupName"]=name;
            var product = _context.CategoryToProducts.Where(c => c.CategoryId == id)
                .Include(c => c.Product).ThenInclude(c=>c.Item).Select(c => c.Product)
                .ToList();
            return View(product);
        }
    }
}
