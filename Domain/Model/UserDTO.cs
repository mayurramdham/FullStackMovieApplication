using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class UserDTO
    {
        public string UserName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string MobileNumber { get; set; }
    }
}
