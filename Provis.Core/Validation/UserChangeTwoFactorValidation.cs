using FluentValidation;
using Provis.Core.DTO.UserDTO;

namespace Provis.Core.Validation
{
    public class UserChangeTwoFactorValidation : AbstractValidator<UserChange2faStatusDTO>
    {
        public UserChangeTwoFactorValidation()
        {
            RuleFor(user => user.Token)
                .NotEmpty()
                .NotNull();
        }
    }
}
