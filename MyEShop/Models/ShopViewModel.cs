using System.Collections.Generic;

namespace MyEShop.Models
{
    public class ShopViewModel
    {
        public List<Product> Products { get; set; }  
        //public List<CategoryWithProducts> CategoriesWithProducts { get; set; }
    }


    //public class CategoryWithProducts
    //{
    //    public Category Category { get; set; }
    //    public List<Product> Products { get; set; }
    //}
    public class DetailsViewMode
    {
        public Product Product { get; set; }
        public  List<Product> Products { get; set; }
        public List<Category> Category { get; set; }


    }

}
