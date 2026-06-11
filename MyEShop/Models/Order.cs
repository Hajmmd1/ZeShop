using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyEShop.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public int  UserId { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public bool IsFinaly { get; set; }


        public Address Address { get; set; }
        public Users Users { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

    }
}
