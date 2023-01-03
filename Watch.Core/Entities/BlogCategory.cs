﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch.Core.Entities
{
    public class BlogCategory :Entity
    {
        public string Name { get; set; }
        public List<Blog> Blogs { get; set; }
    }
}
