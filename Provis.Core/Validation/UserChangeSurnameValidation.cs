using FluentValidation;
using Provis.Core.DTO.userDTO;

namespace Provis.Core.Validation
{
    public class UserChangeSurnameValidation : AbstractValidator<UserChangeSurnameDTO>
    {
        public UserChangeSurnameValidation()
        {
            RuleFor(user => user.Surname)
                .NotNull()
                .Length(3, 50);
        }
    }
}
