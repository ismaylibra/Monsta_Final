using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch.Core.Entities
{
    public class Slider : Entity
    {
        public string? DiscountContent { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public decimal StartPrice { get; set; }
        public string ImageUrl { get; set; }

    }
}
