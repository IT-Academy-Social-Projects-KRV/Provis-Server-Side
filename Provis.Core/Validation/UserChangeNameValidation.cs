using FluentValidation;
using Provis.Core.DTO.userDTO;

namespace Provis.Core.Validation
{
    public class UserChangeNameValidation : AbstractValidator<UserChangeNameDTO>
    {
        public UserChangeNameValidation()
        {
            RuleFor(user => user.Name)
                .NotNull()
                .Length(3, 50);
        }
    }
}
