using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch.Core.Entities
{
    public class AboutWorkers: Entity
    {
        public string Description { get; set; }
        public string FullName { get; set; }
        public string Profession { get; set; }
        public string ImageUrl { get; set; } 

    }
}
