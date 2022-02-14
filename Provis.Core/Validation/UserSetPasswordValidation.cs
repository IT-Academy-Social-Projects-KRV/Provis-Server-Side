using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities.UserEntity;

namespace Provis.Core.Validation
{
    public class UserSetPasswordValidation : AbstractValidator<UserSetPasswordDTO>
    {
        protected readonly UserManager<User> _userManager;

        public UserSetPasswordValidation(UserManager<User> manager)
        {
            _userManager = manager;

            RuleFor(user => user.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]")
                    .WithMessage("{PropertyName} must contain one or more capital letters.")
                .Matches("[a-z]")
                    .WithMessage("{PropertyName} must contain one or more lowercase letters.")
                .Matches(@"\d")
                    .WithMessage("{PropertyName} must contain one or more digits.")
                .Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]")
                    .WithMessage("{PropertyName} must contain one or more special characters.")
                .Matches("^[^£# “”]*$")
                    .WithMessage("{PropertyName} must not contain the following characters £ # “” or spaces.");
        }
    }
}
