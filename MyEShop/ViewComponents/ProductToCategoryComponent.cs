using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyEShop.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyEShop.Data.Repository;
using MyEShop.Models;

namespace MyEShop.ViewComponents
{
    public class ProductToCategoryComponent : ViewComponent
    {

        private readonly IGroupRepository _groupRepository;

        public ProductToCategoryComponent(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = "ProductcategoryGroup")
        {
          
            if (viewName == "CategoryMenu")
                return View("/Views/Components/CategoryMenu.cshtml", _groupRepository.GetShopgroupsAsync());
            else
                return View("/Views/Components/ProductcategoryGroup.cshtml", _groupRepository.GetShopgroupsAsync());
        }

    }
}
