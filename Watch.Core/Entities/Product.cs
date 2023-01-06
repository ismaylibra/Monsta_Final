using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch.Core.Entities
{
    public class Product : Entity
    {
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ShortDescription { get; set; }
        public string MainDescription { get; set; }

        public List<CategoryProduct> CategoryProducts { get; set; }
      

    }
}
