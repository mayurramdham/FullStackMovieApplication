using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? UserName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string UserEmail { get; set; }
        public string? Password { get; set; }
        public string? MobileNumber { get; set; }
        public string? apiKey { get; set; }
        [ForeignKey("Role")]
        public int? RoleId { get; set; }
        public Role? Role { get; set; }

        

    }
}
