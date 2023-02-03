using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watch.Core.Entities;
using Watch.Core.IdentityModels;

namespace Watch.DAL.DAL
{
    public class WatchDbContext : IdentityDbContext<User>
    {
        public WatchDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<CategoryProduct> CategoryProducts { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<AboutPage> AboutPages { get; set; }
        public DbSet<WhyUs> WhyUs { get; set; }
        public DbSet<AboutWorkers> AboutWorkers { get; set; }
        public DbSet<WhyUsShortInfo> whyUsShortInfos { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Setting>().HasIndex(k => k.Key).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
