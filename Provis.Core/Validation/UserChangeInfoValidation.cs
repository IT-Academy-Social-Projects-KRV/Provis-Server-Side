using FluentValidation;
using Provis.Core.DTO.UserDTO;
using Microsoft.AspNetCore.Identity;
using Provis.Core.Entities;

namespace Provis.Core.Validation
{
    public class UserChangeInfoValidation : AbstractValidator<UserChangeInfoDTO>
    {
        protected readonly UserManager<User> _userManager;
        public UserChangeInfoValidation(UserManager<User> manager)
        {
            _userManager = manager;

            RuleFor(user => user.Name)
                .NotNull()
                .Length(3, 50);

            RuleFor(user => user.Surname)
                .NotNull()
                .Length(3, 50);

            RuleFor(user => user.UserName)
                .NotNull()
                .Length(3, 50);
        }
    }
}
