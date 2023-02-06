using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch.Core.Entities
{
    public class WishList : Entity
    {
        public string UserId { get; set; }
        public List<WishListProduct> WishListProducts { get; set; }
    }
}
