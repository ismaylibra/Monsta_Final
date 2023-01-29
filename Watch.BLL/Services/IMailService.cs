using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watch.DAL.Data;

namespace Watch.BLL.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(RequestEmail requestEmail);
    }
}
