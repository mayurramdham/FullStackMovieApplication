using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Interface
{
    public interface IJwtService
    {
        Task<string> Authenticate(int userId, string userEmail, string userName, string Roles,string apiKey);
        Task<string> GenerateToken(int userId, string userEmail, string userName, string Roles,string apiKey);

    }
}
