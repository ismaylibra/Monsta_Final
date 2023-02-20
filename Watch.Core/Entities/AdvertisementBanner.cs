using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch.Core.Entities
{
    public class AdvertisementBanner : Entity
    {
        public string TopTitle { get; set; }
        public string MiddleTitle { get; set; }
        public string BottomTitle { get; set; }
        public string ImageUrl { get; set; }
    }
}
