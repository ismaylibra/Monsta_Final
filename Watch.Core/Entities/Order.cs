using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watch.Core.IdentityModels;

namespace Watch.Core.Entities
{
    public class Order :Entity
    {
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }
        public string Address { get; set; }
        public bool? Status { get; set; }
        public string PhoneNumber { get; set; }
        public List<BasketItem> BasketItems { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
