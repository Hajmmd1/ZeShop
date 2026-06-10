using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client.AppConfig;

namespace MyEShop.Models
{
    public class Users
    {
        [Key]
        public  int UserId { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public DateTime RegisterDate { get; set; } = new DateTime();
        public bool IsAdmin { get; set; }

        public List<Order> Orders { get; set; }
        public List<Address> Addresses { get; set; }

    }
}
