using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.userDTO;
using Provis.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provis.Core.Validation
{
    public class UserLogValidation : AbstractValidator<UserLogDTO>
    {
        public UserLogValidation()
        {
            RuleFor(user => user.Email)
                .NotEmpty()
                .NotNull();

            RuleFor(user => user.Password)
                .NotEmpty()
                .NotNull();
        }
    }
}
