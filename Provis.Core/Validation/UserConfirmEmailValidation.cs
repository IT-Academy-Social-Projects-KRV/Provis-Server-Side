using FluentValidation;
using Provis.Core.DTO.UserDTO;

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
