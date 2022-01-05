using FluentValidation;
using Provis.Core.DTO.workspaceDTO;

namespace Provis.Core.Validation
{
    public class ChangeRoleValidation : AbstractValidator<ChangeRoleDTO>
    {
        public ChangeRoleValidation()
        {
            RuleFor(x => x.RoleId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.WorkspaceId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.UserId)
                .NotNull()
                .NotEmpty();
        }
    }
}
