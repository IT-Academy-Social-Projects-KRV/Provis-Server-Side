using FluentValidation;
using Provis.Core.DTO.UserDTO;

namespace Provis.Core.Validation
{
    public class UserExternalAuthValidation : AbstractValidator<UserExternalAuthDTO>
    {
        public UserExternalAuthValidation()
        {
            RuleFor(x => x.IdToken)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Provider)
                .NotNull()
                .NotEmpty();
        }
    }
}
