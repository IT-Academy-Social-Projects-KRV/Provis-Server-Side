using FluentValidation;
using Provis.Core.DTO.userDTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using Provis.Core.Entities;

namespace Provis.Core.Validation
{
    public class UserRegistrationValidation : AbstractValidator<UserRegDTO>
    {
        private static UserManager<User> _userManager;

        public UserRegistrationValidation(UserManager<User> manager)
        {
            _userManager = manager;

            RuleFor(user => user.Name)
                .NotNull()
                .Length(3, 50);

            RuleFor(user => user.Surname)
                .NotNull()
                .Length(3, 50);

            RuleFor(user => user.Username)
                .NotNull()
                .Length(3, 50)
                .MustAsync(IsUniqueUserName).WithMessage("{PropertyName} already exists.");


            RuleFor(user => user.Email)
                .NotNull()
                .EmailAddress()
                .MustAsync(IsUniqueUserEmail).WithMessage("{PropertyName} already exists.");

            RuleFor(user => user.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("'{PropertyName}' must contain one or more capital letters.")
                .Matches("[a-z]").WithMessage("'{PropertyName}' must contain one or more lowercase letters.")
                .Matches(@"\d").WithMessage("'{PropertyName}' must contain one or more digits.")
                .Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]").WithMessage("'{ PropertyName}' must contain one or more special characters.")
                .Matches("^[^£# “”]*$").WithMessage("'{PropertyName}' must not contain the following characters £ # “” or spaces.");
        }

        private async Task<bool> IsUniqueUserName(string username, CancellationToken cancellationToken)
        {
            var userObject = await _userManager.FindByNameAsync(username);
            if (userObject != null)
            {
                return false;
            }
            return true;
        }

        private async Task<bool> IsUniqueUserEmail(string email, CancellationToken cancellationToken)
        {
            var userObject = await _userManager.FindByEmailAsync(email);
            if (userObject != null)
            {
                return false;
            }
            return true;
        }
    }
}
