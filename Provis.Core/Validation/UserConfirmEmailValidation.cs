using FluentValidation;
using Provis.Core.DTO.userDTO;

namespace Provis.Core.Validation
{
    public class UserConfirmEmailValidation : AbstractValidator<UserConfirmEmailDTO>
    {
        public UserConfirmEmailValidation()
        {
            RuleFor(email => email.ConfirmationCode)
                .NotEmpty()
                .NotNull();
        }
    }
}
