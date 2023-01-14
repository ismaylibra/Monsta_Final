using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch.Core.Entities
{
    public class ProductImage :Entity
    {
        public string Name { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }

    }
}
