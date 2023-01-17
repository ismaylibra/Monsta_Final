using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch.Core.Entities
{
    public class Color : Entity
    {
        public string Name { get; set; }    
        public List<ProductColor> ProductColors { get; set; }
    }
}
