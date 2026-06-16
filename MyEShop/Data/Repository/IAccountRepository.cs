using System.Collections.Generic;
using System.Linq;
using MyEShop.Models;

namespace MyEShop.Data.Repository
{
    public interface IAccountRepository
    {
       
        public Address GetAddressById(int addressId);
        public List<Address> GetAddressesByUserId(int userId);

        public void AddAddress(Address address);
        public void UpdateAddress(Address address);
        public void DeleteAddress(Address address);
        public void SetOrderAddress(int orderId, int addressId);
    }

    public class AccountRepository : IAccountRepository
    {


        private readonly MyShopContext _context;

        public AccountRepository(MyShopContext context)
        {
            _context = context;
        }
     

        public Address GetAddressById(int addressId)
        {
            return _context.Addresses.FirstOrDefault(c => c.AddressId == addressId);
        }

        public List<Address> GetAddressesByUserId(int userId)
        {
            return _context.Addresses.Where(c => c.UserId == userId).ToList();
        }

        public void AddAddress(Address address)
        {
            _context.Add(address);
            _context.SaveChanges();
        }

        public void UpdateAddress(Address address)
        {
          
                _context.Addresses.Update(address);
                _context.SaveChanges();


        }

        public void DeleteAddress(Address address)
        {
            _context.Addresses.Remove(address);
            _context.SaveChanges();
        }

        public void SetOrderAddress(int orderId, int addressId)
        {
            var orderID = _context.Orders.Find(orderId);
            var addres = GetAddressById(addressId);
            if (orderID!=null&&addres!=null)
            {
                if (orderID.UserId == addres.UserId)
                {
                    orderID.AddressId = addressId;
                    _context.SaveChanges();

                }
            }
            



        }
    }
}
