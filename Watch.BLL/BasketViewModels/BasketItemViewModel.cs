using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watch.Core.Entities;

namespace Watch.BLL.BasketViewModels
{
    public class BasketItemViewModel
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
