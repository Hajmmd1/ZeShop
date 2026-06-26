using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Data;
using MyEShop.Models;

namespace MyEShop.Pages.Admin
{
    public class ProductModel : PageModel
    {
        private readonly MyShopContext _context;

        public List<Category> Categories { get; set; }
        [BindProperty]
        public List<int> SelectCategoryId { get; set; }
        public ProductModel(MyShopContext context)
        {
            _context = context;
        }
        [BindProperty]
        public AddOrEditProduct AddOrEditProduct { get; set; }
        public IEnumerable<Product> Products { get; set; }
        [BindProperty]

        public Product Product { get; set; }






        public void OnGet(int? id)
        {
            // بارگذاری دسته‌بندی‌ها و محصولات برای جدول
            Categories = _context.Categories.ToList();
            Products = _context.Products
                .Include(p => p.Item)
                .Include(p => p.CategoryToProducts)
                .ThenInclude(cp => cp.Category)
                .ToList();

            if (id == null || id == 0)
            {
                // حالت افزودن
                AddOrEditProduct = new AddOrEditProduct(); // Id = 0
                SelectCategoryId = new List<int>();
            }
            else
            {
                // حالت ویرایش
                var product = _context.Products
                    .Include(p => p.Item)
                    .Include(p => p.CategoryToProducts)
                    .FirstOrDefault(p => p.Id == id);

                if (product != null)
                {
                    AddOrEditProduct = new AddOrEditProduct
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Item.Price,
                        QuantityInStock = product.Item.QuantityInStock
                    };
                    SelectCategoryId = product.CategoryToProducts.Select(cp => cp.CategoryId).ToList();
                }
                else
                {
                    AddOrEditProduct = new AddOrEditProduct();
                    SelectCategoryId = new List<int>();
                }
            }
        }

        public IActionResult OnPost()
        {
            if (SelectCategoryId == null || SelectCategoryId.Count == 0)
            {
                Categories = _context.Categories.ToList();
                ModelState.AddModelError("SelectCategoryId", "حداقل یک دسته‌بندی باید انتخاب شود.");
                Products = _context.Products.Include(p => p.Item).ToList();
                return Page();
            }

            if (!ModelState.IsValid)
            {
                Products = _context.Products.Include(p => p.Item).ToList();
                Categories = _context.Categories.ToList();
                return Page();
            }

            // ویرایش
            if (AddOrEditProduct.Id > 0)
            {
                var existingProduct = _context.Products
                    .Include(p => p.Item)
                    .Include(p => p.CategoryToProducts)
                    .FirstOrDefault(p => p.Id == AddOrEditProduct.Id);

                if (existingProduct == null) return NotFound();

                existingProduct.Name = AddOrEditProduct.Name;
                existingProduct.Description = AddOrEditProduct.Description;
                existingProduct.Item.Price = AddOrEditProduct.Price;
                existingProduct.Item.QuantityInStock = AddOrEditProduct.QuantityInStock;

                // بروزرسانی دسته‌بندی‌ها
                _context.RemoveRange(existingProduct.CategoryToProducts);
                foreach (var catId in SelectCategoryId)
                {
                    _context.Add(new CategoryToProducts { ProductId = existingProduct.Id, CategoryId = catId });
                }

                if (AddOrEditProduct.Picture?.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", $"{existingProduct.Id}.jpg");
                    using var stream = new FileStream(filePath, FileMode.Create);
                    AddOrEditProduct.Picture.CopyTo(stream);
                }

                _context.SaveChanges();
                return RedirectToPage();
            }

            // افزودن
            var item = new Item { Price = AddOrEditProduct.Price, QuantityInStock = AddOrEditProduct.QuantityInStock };
            _context.Items.Add(item);
            _context.SaveChanges();

            var newProduct = new Product
            {
                Name = AddOrEditProduct.Name,
                Description = AddOrEditProduct.Description,
                ItemId = item.Id
            };
            _context.Products.Add(newProduct);
            _context.SaveChanges();

            foreach (var catId in SelectCategoryId)
            {
                _context.Add(new CategoryToProducts { ProductId = newProduct.Id, CategoryId = catId });
            }

            if (AddOrEditProduct.Picture?.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", $"{newProduct.Id}.jpg");
                using var stream = new FileStream(filePath, FileMode.Create);
                AddOrEditProduct.Picture.CopyTo(stream);
            }

            _context.SaveChanges();
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
         var product=  _context.Products.Include(c=>c.Item)
             .Include(c=>c.CategoryToProducts).FirstOrDefault(c=>c.Id==id);

         if (product == null)
             return NotFound();

         if (product.CategoryToProducts.Any())
             _context.RemoveRange(product.CategoryToProducts);

         if (product.Item!=null)
         {
             _context.Items.Remove(product.Item);
         }


         _context.Products.Remove(product);

            _context.SaveChanges();


         var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", $"{product.Id}.jpg");
         if (System.IO.File.Exists(filePath))
         {
             System.IO.File.Delete(filePath);
         }

         return RedirectToPage();

        }
    }
}
