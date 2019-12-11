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
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace AntiAuc.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationContext _context;
        private IHostingEnvironment _environment;
        public ProductController(ApplicationContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Info(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Product product = _context.Products.Include(p => p.Users).ThenInclude(pu => pu.User).FirstOrDefault(x => x.ProductId == id);
                ViewBag.product = product;
                return View();
            }

        }

        [Authorize(Roles = "admin , user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Info(int? id, BetModel model)
        {

            Product product = _context.Products.Include(p => p.Users).ThenInclude(pu => pu.User).FirstOrDefault(x => x.ProductId == id);
            if (ModelState.IsValid)
            {
                User currentUser = _context.Users.Include(p => p.Products).ThenInclude(pu => pu.Product).FirstOrDefault(x => x.Email == User.Identity.Name);
                if (product.Users.Last().User.Email != currentUser.Email)
                {
                    if (model.NewPrice >= product.CurrentPrice + product.CurrentPrice / 10)
                    {
                        if (currentUser.Deposit >= model.NewPrice)
                        {
                            if (product.DateOfEnding >= DateTime.Now)
                            {
                                if (product.Users.Last().User.Email != product.Users.First().User.Email)
                                {
                                    User lastUser = product.Users.Last().User;
                                    lastUser.Deposit += product.CurrentPrice;
                                    product.CurrentPrice = model.NewPrice;
                                    currentUser.Deposit -= product.CurrentPrice;

                                    UserProduct usp = _context.UserProducts.First(x => x.UserId == lastUser.Id && x.ProductId == product.ProductId);
                                    lastUser.Products.Remove(usp);
                                    _context.SaveChanges();

                                    usp.UserId = currentUser.Id;
                                    currentUser.Products.Insert(0, usp);

                                    _context.Users.Update(lastUser);
                                    _context.Users.Update(currentUser);
                                    _context.Products.Update(product);
                                    await _context.SaveChangesAsync();

                                    User user = _context.Users.Include(p => p.Products).ThenInclude(pu => pu.Product).FirstOrDefault(x => x.Email == User.Identity.Name);
                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {
                                    product.CurrentPrice = model.NewPrice;
                                    currentUser.Deposit -= product.CurrentPrice;
                                    UserProduct newusp = new UserProduct() { UserId = currentUser.Id, ProductId = product.ProductId };
                                    _context.UserProducts.Add(newusp);
                                    _context.Users.Update(currentUser);
                                    _context.Products.Update(product);
                                    await _context.SaveChangesAsync();

                                    User user = _context.Users.Include(p => p.Products).ThenInclude(pu => pu.Product).FirstOrDefault(x => x.Email == User.Identity.Name);
                                    return RedirectToAction("Index","Home");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("","Lot isn't available");
                            }
                        }
                        else
                        {

                            ModelState.AddModelError("NewPrice", "* Not enough money");
                        }
                    }
                    else if(model.NewPrice == 0)
                    {
                        ModelState.AddModelError("NewPrice", "* Enter sum");
                    }
                    else
                    {
                        ModelState.AddModelError("NewPrice", "* Bet isn't big enough");
                    }
                    ViewBag.product = product;
                    return View(model);

                }
                else
                {
                    return Content($"{id}");
                }
            }
            else
            {
                ViewBag.prodcut = product;
                return View(model);
            }
        }

        [Authorize(Roles = "admin , user")]
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [Authorize(Roles = "admin , user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                Product pr = new Product
                {
                    ProductName = model.ProductName,
                    ShortDescription = model.ShortDescription,
                    Description = model.Description,
                    CurrentPrice = model.StartPrice,
                    DateOfCreation = model.DateOfCreation,
                    DateOfEnding = model.DateOfCreation.AddDays(model.DateOfEnding),
                };
                if (model.File != null)
                {
                    string path = "/images/" + model.File.FileName;
                    using (var fileStream = new FileStream(_environment.WebRootPath + path, FileMode.Create))
                    {
                        await model.File.CopyToAsync(fileStream);
                    }
                    pr.Image = path;
                }
               
                _context.Products.Add(pr);
                await _context.SaveChangesAsync();

                UserProduct up = new UserProduct
                {
                    UserId = user.Id,
                    ProductId = pr.ProductId,
                };

                _context.UserProducts.Add(up);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            };
            return View(model);
        }

        public async Task<IActionResult> CreateMessage(Message model)
        {
            User currentUser = _context.Users.Include(p => p.Products).ThenInclude(pu => pu.Product).FirstOrDefault(x => x.Email == User.Identity.Name);
            model.Sender = currentUser;
            currentUser.Messages.Add(model);
            _context.Message.Add(model);
            _context.Users.Update(currentUser);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id != null)
            {
                Product product = _context.Products.Find(id);
                ViewBag.Product = product;
               
            }
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditProductModel model)
        {
            Product product = _context.Products.Find(model.ProductId);
            if (ModelState.IsValid)
            {
                if (model.File != null)
                {
                    string path = "/images/" + model.File.FileName;
                    using (var fileStream = new FileStream(_environment.WebRootPath + path, FileMode.Create))
                    {
                        await model.File.CopyToAsync(fileStream);
                    }
                    product.Image = path;
                }
                else
                {
                    product.Image = model.Image;
                }
                product.ShortDescription = model.ShortDescription;
                product.DateOfCreation = model.DateOfCreation;
                product.DateOfEnding = model.DateOfEnding;
                product.CurrentPrice = model.CurrentPrice;
                product.ShortDescription = model.ShortDescription;
                product.ProductName = model.ProductName;
                product.IsAvailable = model.IsAvailable;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","Home");
            }
            else
            {    
                ViewBag.Product = product;
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            Product product = _context.Products.Include(c => c.Users).ThenInclude(sc => sc.User).FirstOrDefault(u => u.ProductId == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ModeratorConfig", "Account");
        }
    }
}
