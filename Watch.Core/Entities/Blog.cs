using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch.Core.Entities
{
    public class Blog :Entity
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime PublishTime { get; set; }
        public string ShortContent { get; set; }
        public string SpecifiedContent { get; set; }
        public string MainContent { get; set; }
        public BlogCategory Category { get; set; }
        public int CategoryId { get; set; }
    }
}
