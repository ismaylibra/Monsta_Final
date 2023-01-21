using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using Watch.BLL.Data;
using Watch.Core.IdentityModels;
using Watch.DAL.DAL;

namespace WatchECommerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<WatchDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                   
            });

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<WatchDbContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            Constants.RootPath = builder.Environment.WebRootPath;
            Constants.SliderPath = Path.Combine(Constants.RootPath, "assets", "img", "slider");
            Constants.BannerPath = Path.Combine(Constants.RootPath, "assets", "img", "bg");
            Constants.BlogPath = Path.Combine(Constants.RootPath, "assets", "img", "blog");
            Constants.ProductImagePath = Path.Combine(Constants.RootPath, "assets", "img", "product");
            Constants.AboutPath = Path.Combine(Constants.RootPath, "assets", "img", "about");
            Constants.UserPath = Path.Combine(Constants.RootPath, "assets", "img", "user");


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
                );
                endpoints.MapControllerRoute(
                  name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });




            app.Run();

        }
    }
}