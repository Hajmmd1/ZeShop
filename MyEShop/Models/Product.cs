using System.Collections.Generic;
using System.Security.AccessControl;

namespace MyEShop.Models
{
    public class Product
    {
        //public Product()
        //{
        //    Categories=new List<Category>();
        //}
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ItemId { get; set; }
        

        public ICollection<CategoryToProducts> CategoryToProducts { get; set; }
        public Item Item { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }


    }
}
