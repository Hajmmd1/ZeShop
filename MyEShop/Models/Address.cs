using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MyEShop.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }
        [Required]
     
        public  int UserId { get; set; }
        [Required]
        [MaxLength(500)]
        public string City { get; set; }
        [Required]
        [MaxLength(15)]
        public string PostalCode { get; set; }

        public Users Users { get; set; }

    }
}
