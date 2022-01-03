using FluentValidation;
using Provis.Core.DTO.UserDTO;

namespace Provis.Core.Validation
{
    public class UserChangeTwoFactorValidation : AbstractValidator<UserChangeTwoFactorDTO>
    {
        public UserChangeTwoFactorValidation()
        {
            RuleFor(user => user.IsTwoFactor)
                .NotEmpty();
        }
    }
}
