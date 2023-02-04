using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watch.Core.IdentityModels;

namespace Watch.Core.Entities
{
    public class BasketItem :Entity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
