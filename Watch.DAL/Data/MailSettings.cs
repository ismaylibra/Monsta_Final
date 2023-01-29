using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch.DAL.Data
{
    public class MailSettings
    {
        public string Mail { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
    }
}
