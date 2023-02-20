using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using Watch.Core.Entities;
using Watch.Core.IdentityModels;
using Watch.DAL.DAL;
using WatchECommerce.ViewModels;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Watch.BLL.Services;

namespace WatchECommerce.Areas.Admin.Controllers
{
    public class OrderController : BaseController
    {
        private readonly WatchDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IMailService _mailManager;
        private readonly IConfiguration _config;

        public OrderController(WatchDbContext dbContext, UserManager<User> userManager, IMailService mailManager, IConfiguration config)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _mailManager = mailManager;
            _config = config;
        }


        public async Task<IActionResult> Index()
        {
            var orders = await _dbContext.Orders
                .Include(order => order.OrderItems)
                .ThenInclude(order => order.Product)
                .ToListAsync();

            var model = new List<OrderViewModel>();

            if (orders is not null)
            {
                foreach (var order in orders)
                {
                    var user = await _userManager.FindByIdAsync(order.UserId);
                    int count = 0;
                    foreach (var item in order.OrderItems)
                    {
                        count += item.Count;
                    }
                

                    model.Add(new OrderViewModel
                    {
                        Name = user.UserName,
                        Id = order.Id,
                        Time = order.CreateTime,
                        Status = order.Status,
                        Count = count,
                        Items = order.OrderItems

                    });


                }
            }

            return View(model);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _dbContext.Orders.Where(order => order.Id == id)
              .Include(order => order.OrderItems)
              .ThenInclude(order => order.Product)
              .FirstOrDefaultAsync();

            if (order == null) return NotFound();


            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderStatus(int? id, bool? Status)
        {
            if (id == null) return NotFound();

            var order = await _dbContext.Orders
             .Where(order => order.Id == id)
             .FirstOrDefaultAsync();

            var user = await _userManager.FindByIdAsync(order.UserId);

            if (order == null) return NotFound();
            order.Status = Status;


            EmailViewModel email = _config.GetSection("Email").Get<EmailViewModel>();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email.SenderEmail, email.SenderName);
            mail.To.Add(user.Email);
            if (Status == false)
            {
                mail.Subject = $"Canceled";
                mail.Body = $"Dear {user.UserName.ToUpper()},Your Order is Succesfully Accepted";

            }
            else
            {
                mail.Subject = "Success";
                mail.Body = $"Dear {user.UserName.ToUpper()}, Your Order is Cancelled";

            }
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = email.Server;
            smtp.Port = email.Port;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(email.SenderEmail, email.Password);
            smtp.Send(mail);

            await _dbContext.SaveChangesAsync();

            return (RedirectToAction(nameof(Index)));
        }


    }
}
