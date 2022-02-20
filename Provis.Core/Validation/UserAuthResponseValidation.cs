using FluentValidation;
using Provis.Core.DTO.UserDTO;

namespace Provis.Core.Validation
{
    public class UserAuthResponseValidation : AbstractValidator<UserAuthResponseDTO>
    {
        public UserAuthResponseValidation()
        {
            RuleFor(x => x.RefreshToken)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Token)
                .NotNull()
                .NotEmpty();
        }
    }
}
