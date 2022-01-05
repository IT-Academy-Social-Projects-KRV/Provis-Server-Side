using FluentValidation;
using Provis.Core.DTO.UserDTO;

namespace Provis.Core.Validation
{
    public class UserTwoFactorLoginValidation : AbstractValidator<UserTwoFactorDTO>
    {
        public UserTwoFactorLoginValidation()
        {
            RuleFor(user => user.Token)
                .NotEmpty()
                .NotNull();

            RuleFor(user => user.Provider)
                .NotEmpty()
                .NotNull();

            RuleFor(user => user.Email)
                .NotEmpty()
                .NotNull();
        }
    }
}
