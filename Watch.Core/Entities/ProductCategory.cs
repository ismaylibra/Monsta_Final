using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch.Core.Entities
{
    public class ProductCategory :Entity
    {
        public string Name { get; set; }
        public List<CategoryProduct> CategoryProducts { get; set; }
    }
}
