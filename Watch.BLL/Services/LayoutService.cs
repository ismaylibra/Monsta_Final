using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watch.Core.Entities;
using Watch.DAL.DAL;

namespace Watch.BLL.Services
{
    public class LayoutService
    {
        private readonly WatchDbContext _dbContext;

        public LayoutService(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Setting> GetSettings()
        {
            List<Setting> settings = _dbContext.Settings.ToList();
            return settings;
        }
    }
}
