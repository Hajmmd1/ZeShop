using Microsoft.AspNetCore.Http;

namespace MyEShop.Models
{
    public class AddOrEditProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int  QuantityInStock { get; set; }
        public IFormFile Picture { get; set; }

    }
}
