using FluentValidation;
using Provis.Core.DTO.UserDTO;

namespace Provis.Core.Validation
{
    public class UserLogValidation : AbstractValidator<UserLoginDTO>
    {
        public UserLogValidation()
        {
            RuleFor(user => user.Email)
                .NotEmpty()
                .NotNull();

            RuleFor(user => user.Password)
                .NotEmpty()
                .NotNull();
        }
    }
}
