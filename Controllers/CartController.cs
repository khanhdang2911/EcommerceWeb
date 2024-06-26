using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecommerce_website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce_website.Controllers
{
    // [Route("[controller]")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly AppDbContext _context;
        public CartController(ILogger<CartController> logger,AppDbContext context)
        {
            _logger = logger;
            _context=context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(int productId,int quantity=1)
        {
            int UserId=int.Parse(User.Claims.FirstOrDefault(c=>c.Type=="Id").Value);
            ProductUsers productUser=new ProductUsers();
            var kq=_context.productUsers.Where(p=>p.UsersId==UserId&&p.ProductId==productId).FirstOrDefault();
            if(kq==null)
            {
                productUser.UsersId=UserId;
                productUser.ProductId=productId;
                productUser.ProductQuantity=quantity;
                await _context.productUsers.AddAsync(productUser);
            }
            else{
                _context.Entry(kq).State = EntityState.Modified;
                kq.ProductQuantity+=quantity;
            }
            await _context.SaveChangesAsync();

            int cartCount=0;
            cartCount=_context.productUsers.Where(c=>c.UsersId==UserId&&c.Status==false).ToList().Count;

            return Json(new { success = true, message = "Product added to cart successfully", cartCount = cartCount });

        }
        public IActionResult Detail()
        {
            return View();
        }
        [HttpPost]
        public async  Task<IActionResult> Checkout(int UsersId, string Address,decimal TotalMoney, string DeliveryMethod,List<int> productListId)
        {
            Order order=new Order();
            order.UsersId=UsersId;
            order.Address=Address;
            order.TotalMoney=TotalMoney;
            order.DeliveryMethod=DeliveryMethod;
            order.DateBuy=DateTime.Now;
            var newOrder=await _context.orders.AddAsync(order);
            var removeItems=_context.productUsers.Where(p=>p.UsersId==UsersId).ToList();
            foreach(var item in productListId)
            {
                var productUser=_context.productUsers.First(p=>p.UsersId==UsersId&&p.ProductId==item);
                _context.Entry(productUser).State=EntityState.Modified;
                productUser.Status=true;
            }
            await _context.SaveChangesAsync();

           
            
            return RedirectToAction("Home","Category");
        }
        [HttpGet]
        public async Task<IActionResult> RemoveItem(int productId)//Chuuyen doi trang thai san pham da dat hang
        {
            var UserId=User.Claims.FirstOrDefault(c=>c.Type=="Id").Value;
            if(_context.productUsers.Any(p=>p.UsersId==int.Parse(UserId)&&p.ProductId==productId))
            {
                var removeItem=await _context.productUsers.Where(p=>p.UsersId==int.Parse(UserId)&&p.ProductId==productId).FirstAsync();
                _context.productUsers.Remove(removeItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Detail");
        }
    }
}