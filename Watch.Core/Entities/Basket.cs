using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch.Core.Entities
{
    public class Basket : Entity
    {

        public string UserId { get; set; }
        public List<BasketProduct> BasketProducts { get; set; }
    }
}
