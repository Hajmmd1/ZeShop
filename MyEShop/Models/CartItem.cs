using System.Security.AccessControl;

namespace MyEShop.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }   

        public int CartId { get; set; }      
        public int ItemId { get; set; }      
        public int Quantity { get; set; }

       
        public Cart Cart { get; set; }
        public Item Item { get; set; }

        public decimal GetTotalPrice() => Item?.Price * Quantity ?? 0;
    }
}
