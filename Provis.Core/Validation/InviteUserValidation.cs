using FluentValidation;
using Provis.Core.DTO.workspaceDTO;

namespace Provis.Core.Validation
{
    public class InviteUserValidation : AbstractValidator<InviteUserDTO>
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
