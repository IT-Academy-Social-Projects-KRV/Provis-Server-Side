using FluentValidation;
using Provis.Core.DTO.workspaceDTO;

namespace Provis.Core.Validation
{
    public class WorkspaceUpdateValidation : AbstractValidator<WorkspaceUpdateDTO>
    {
        public WorkspaceUpdateValidation()
        {
            RuleFor(workspace => workspace.Name)
                .NotEmpty()
                .NotNull()
                .Length(1, 50)
                .Matches("^[^£# “”]*$").WithMessage("{PropertyName} must not contain the following characters £ # “” or spaces.");

            RuleFor(workspace => workspace.Description)
                .NotNull()
                .Length(0, 100)
                .Matches("^[^£# “”]*$").WithMessage("{PropertyName} must not contain the following characters £ # “” or spaces.");
        }
    }
}
