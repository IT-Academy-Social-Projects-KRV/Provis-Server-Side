using FluentValidation;
using Provis.Core.DTO.userDTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using Provis.Core.Entities;

namespace Provis.Core.Validation
{
    public class UserChangeUsernameValidation : AbstractValidator<UserChangeUsernameDTO>
    {

        private static UserManager<User> _userManager;
        public UserChangeUsernameValidation(UserManager<User> manager)
        {
            _userManager = manager;
            RuleFor(user => user.Username)
                .NotNull()
                .Length(3, 50)
                .MustAsync(IsUniqueUserName).WithMessage("{PropertyName} already exists.");
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
    }
}
