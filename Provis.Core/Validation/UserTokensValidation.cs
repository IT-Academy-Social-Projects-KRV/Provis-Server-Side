using FluentValidation;
using Provis.Core.DTO.userDTO;

namespace Provis.Core.Validation
{
    class UserTokensValidation: AbstractValidator<UserTokensDTO>
    {
        public UserTokensValidation()
        {
            RuleFor(user => user.Token)
               .NotEmpty()
               .NotNull();

            RuleFor(user => user.RefreshToken)
                .NotEmpty()
                .NotNull();
        }
    }
}
