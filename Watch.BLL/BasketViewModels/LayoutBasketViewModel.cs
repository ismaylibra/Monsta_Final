using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watch.Core.Entities;

namespace Watch.BLL.BasketViewModels
{
    public class LayoutBasketViewModel
    {
        public List<BasketItemViewModel> BasketItemViewModels { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
