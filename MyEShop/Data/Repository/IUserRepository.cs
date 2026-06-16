using System.Linq;
using MyEShop.Models;

namespace MyEShop.Data.Repository
{
    public interface IUserRepository
    {
        bool IsExsistUserByUserName(string userName);
        Users GetUsersForLogin(string userName,string password);
        public Users GetUsrById(int usrId);
        void AdUser(Users users);

    }

    public class UserRepository : IUserRepository
    {
        private readonly MyShopContext _context;

        public UserRepository(MyShopContext context)
        {
            _context = context;
        }

        public bool IsExsistUserByUserName(string userName)
        {
            return _context.User.Any(c => c.UserName == userName);
        }

        public Users GetUsersForLogin(string userName, string password)
        {
            return _context.User.SingleOrDefault(c=>c.UserName==userName&&c.Password==password);
        }

        public void AdUser(Users users)
        {
            _context.Add(users);
            _context.SaveChanges();
        }

        public Users GetUsrById(int usrId)
        {
            return _context.User.FirstOrDefault(c => c.UserId == usrId);
        }
    }

    
}
