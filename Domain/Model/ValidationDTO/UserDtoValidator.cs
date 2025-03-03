using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.ValidationDTO
{
    public class UserDtoValidator: AbstractValidator<UserDTO>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.UserEmail).EmailAddress().NotEmpty();
            RuleFor(x => x.UserName).Length(2, 15).ToString();
            RuleFor(p => p.Password).Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.");
            RuleFor(p => p.Password).Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.");
            RuleFor(p => p.Password).Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
            RuleFor(m => m.MobileNumber).NotEmpty().Length(10).WithMessage("Invalid phone number.");
            RuleFor(x => x.Password).Matches(@"[\!\?\*\.]*$").WithMessage("Your password must contain at least one (!? *.).");
            RuleFor(x => x.RoleId).NotEmpty();
            RuleFor(x => x.RoleId).Must(ContainOneOrTwo);
        }  
           private bool ContainOneOrTwo(int id) { var idString = id.ToString(); return idString.Contains('1') || idString.Contains('2');   }
    }
}
