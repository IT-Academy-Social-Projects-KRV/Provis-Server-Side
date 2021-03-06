using FluentValidation;
using Provis.Core.DTO.WorkspaceDTO;

namespace Provis.Core.Validation
{
    public class InviteUserValidation : AbstractValidator<WorkspaceInviteUserDTO>
    {
        public InviteUserValidation()
        {
            RuleFor(invite => invite.WorkspaceId)
                .NotEmpty()
                .NotNull();

            RuleFor(invite => invite.UserEmail)
                .NotEmpty()
                .NotNull();
        }
    }
}
