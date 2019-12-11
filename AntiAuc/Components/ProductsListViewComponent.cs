using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AntiAuc.Models;

namespace AntiAuc.Components
{
    public class ProductsListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<Product> collection, User user)
        {
            ViewBag.UserBag = user;
            return View(collection);
            
        }
    }
}
