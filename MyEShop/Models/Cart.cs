using System;
using System.Collections.Generic;
using System.Linq;

namespace MyEShop.Models
{
    public class Cart
    {
        public int CartId { get; set; }              
        public int? OrderId { get; set; }
        //public string UserId { get; set; } = null;        
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        public void AddItem(CartItem item)
        {
            if (CartItems.Exists(c=>c.Item.Id==item.Item.Id))
            {
                CartItems.Find(i => i.Item.Id == item.Item.Id).Quantity += 1;
            }
            else
            {
                CartItems.Add(item);
            }

        }

        public void RemoveItem(int itemId)
        {
            var item = CartItems.SingleOrDefault(c => c.Item.Id == itemId);
            if (item?.Quantity<=1)
            {
                CartItems.Remove(item);
            }
            else if (item!=null)
            {
                item.Quantity -= 1;
            }
            


        }
      

    }
}
