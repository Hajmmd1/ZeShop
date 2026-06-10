using MyEShop.Models;

namespace MyEShop.Data.Repository
{
    public interface IAccountRepository
    {
        public Users GetUsrById(int usrId);
        public Address GetAddressById(int usrId);
        public void AddAddress(Address address);
        public void UpdateAddress(Address address);
        public void DeleteAddress(int addressId);
    }
}
