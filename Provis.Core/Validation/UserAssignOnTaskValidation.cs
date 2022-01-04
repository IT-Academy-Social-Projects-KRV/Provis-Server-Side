using FluentValidation;
using Provis.Core.DTO.userDTO;

namespace Provis.Core.Validation
{
    public class UserAssignOnTaskValidation: AbstractValidator<AssignedUserOnTaskDTO>
    {
        public UserAssignOnTaskValidation()
        {
            RuleFor(user => user.UserId)
               .NotEmpty()
               .NotNull();

            RuleFor(user => user.RoleTagId)
               .NotEmpty()
               .NotNull();
        }
    }
}
