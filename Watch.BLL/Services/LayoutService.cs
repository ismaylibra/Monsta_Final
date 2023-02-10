using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watch.Core.Entities;
using Watch.DAL.DAL;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Watch.BLL.Services
{
    public class LayoutService
    {
        private readonly WatchDbContext _dbContext;
        private readonly IHttpContextAccessor _http;

        public LayoutService(WatchDbContext dbContext, IHttpContextAccessor http)
        {
            _dbContext = dbContext;
            _http = http;
        }

        public List<Setting> GetSettings()
        {
            List<Setting> settings = _dbContext.Settings.ToList();
            return settings;
        }

      

    }
}
