using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiAuc.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using AntiAuc.Models;

namespace AntiAuc.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext _context;
        public AccountController(ApplicationContext context)
        {
            _context = context;
        }

        [Route("/Account/Edit/{id?}")]
        [Authorize(Roles = "user, admin")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id != null)
            {
                User user = _context.Users.FirstOrDefault(x => x.Id == id);
                if (User.IsInRole("admin"))
                    ViewBag.User = user;
                else if (user.Email == User.Identity.Name)
                {
                    ViewBag.User = user;
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            else
                ViewBag.User = _context.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult ModeratorConfig()
        {
            List<User> users = _context.Users.Include(c => c.Products).ThenInclude(sc => sc.Product).ToList();
            return View(users);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModeratorConfig(EditModel model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }
            else
                return View(model);
        }



        [Authorize(Roles = "user, admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditModel model)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
            if (ModelState.IsValid)
            {
                if (User.IsInRole("user"))
                    if (model.OldPassword == user.Password)
                    {
                        user.Password = model.NewPassword;
                        _context.Users.Update(user);
                        await _context.SaveChangesAsync();
                        return RedirectToAction($"Profile/{user.Id}", "Account");
                    }
                    else
                    {
                        ViewBag.User = user;
                        ModelState.AddModelError("", "* Old password error");
                        return View(model);

                    }
                else
                {
                    user.Email = model.Email;
                    user.Password = model.NewPassword;
                    user.RoleId = model.RoleId;
                    user.Deposit = model.Deposit;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction($"ModeratorConfig", "Account");
                }

            }
            else
            {
                ViewBag.User = user;
                return View(model);
            }
        }
        



        [HttpGet]
        public async Task<IActionResult> Profile(int? id)
        {
            User user;
            List<Product> products;

            if (id == null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    user = await _context.Users.Include(u => u.Role).Include(c => c.Products).ThenInclude(sc => sc.Product).FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                    ViewBag.UserBag = user;

                    products = user.Products.Select(x => x.Product).ToList();
                    await Authenticate(user);
                    return View(products);
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            else
            {
                user = await _context.Users.Include(u => u.Role).Include(c => c.Products).ThenInclude(sc => sc.Product).FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    ViewBag.UserBag = user;

                    products = user.Products.Select(x => x.Product).ToList();
                    return View(products);
                }
                else
                    return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    user = new User { Email = model.Email, Password = model.Password };
                    Role userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                    if (userRole != null)
                        user.Role = userRole;

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (!User.Identity.IsAuthenticated)
                return View();
            else
                return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            User user = _context.Users.Include(c => c.Products).ThenInclude(sc => sc.Product).FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ModeratorConfig", "Account");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        private async Task Authenticate(User user)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (user != null)
            {
                // создаем один claim
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
                };
                // создаем объект ClaimsIdentity
                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                // установка аутентификационных куки
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
            }
        }


    }
}
