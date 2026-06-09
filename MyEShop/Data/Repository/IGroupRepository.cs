using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models;

namespace MyEShop.Data.Repository
{
    public interface IGroupRepository
    {
        IEnumerable<Category> GetCategories();
        IEnumerable<ShopgroupViewModel> GetShopgroupsAsync();
    }

    public class GroupRepository:IGroupRepository
    {
        private readonly MyShopContext _context;

        public GroupRepository(MyShopContext context)
        {
            _context = context;
        }
        public IEnumerable<Category> GetCategories()
        {
          return _context.Categories.ToList();
        }

        public IEnumerable<ShopgroupViewModel> GetShopgroupsAsync()
        {

          return    _context.Categories.Select(c=>new ShopgroupViewModel()
          {
              GroupId = c.Id,
              Name = c.Name,
              ProductCount = _context.CategoryToProducts.Count(g=>g.CategoryId==c.Id)



          }).ToList();
        }
    }

}
