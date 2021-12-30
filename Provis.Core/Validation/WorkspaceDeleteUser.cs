using FluentValidation;
using Provis.Core.DTO.workspaceDTO;

namespace Provis.Core.Validation
{
    public class WorkspaceDeleteUser : AbstractValidator<DeleteUserDTO>
    {
        public WorkspaceDeleteUser()
        {
            RuleFor(user => user.UserEmail)
                .NotNull()
                .NotEmpty();
        }
    }
}
