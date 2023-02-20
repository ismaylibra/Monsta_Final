using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch.Core.Entities
{
    public class Product : Entity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ShortDescription { get; set; }
        public string MainDescription { get; set; }
        public Brand Brand { get; set; }
        public int BrandId { get; set; }

        public string MainImageUrl { get; set; }

        public decimal? DiscountPrice { get; set; }

        public List<ProductImage> ProductImages { get; set; }
        public List<CategoryProduct> CategoryProducts { get; set; }
        public List<ProductColor> ProductColors { get; set; }
        public List<WishListProduct> WishListProducts { get; set; }
        public bool IsLike { get; set; }



    }
}
