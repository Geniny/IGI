using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AntiAuc.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace AntiAuc.Controllers
{
    public class HomeController : Controller
    {
        ApplicationContext db;
        public HomeController(ApplicationContext context)
        {
            db = context;
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }


        [HttpGet]
        public async Task<IActionResult> Index(string sort)
        {
            if (sort == "Price")
                return View(db.Products.ToList().Where(x => x.IsAvailable == true).OrderBy(i => i.CurrentPrice));
            else if(sort == "Name")
            {
                return View(db.Products.ToList().Where(x => x.IsAvailable == true).OrderBy(i => i.ProductName));
            }
            else
            {
                IEnumerable<Product> products = db.Products.Include(p => p.Users).ThenInclude(t => t.User).Where(x => x.IsAvailable == true);
                foreach (var item in products)
                {
                    if (item.DateOfEnding <= DateTime.Now)
                    {
                        item.IsAvailable = false;
                        if(item.Users.First() != item.Users.Last())
                        {
                            User owner = db.Users.Find(item.Users.First().User.Id);
                            owner.Deposit += item.CurrentPrice;
                            db.Users.Update(owner);
                        }
                        
                        db.Products.Update(item);
                        
                    }
                }
                await db.SaveChangesAsync();
                return View(db.Products.Where(x => x.IsAvailable == true));
            }  

        }
        

        [Authorize(Roles = "admin")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
    }
}
