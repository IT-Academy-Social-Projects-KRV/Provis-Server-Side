using FluentValidation;
using Provis.Core.DTO.workspaceDTO;

namespace Provis.Core.Validation
{
    public class WorkspaceCreateValidation : AbstractValidator<WorkspaceCreateDTO>
    {
        public WorkspaceCreateValidation()
        {
            RuleFor(workspace => workspace.Name)
                .NotEmpty()
                .NotNull()
                .Length(1, 70)
                .Matches("^[^£#“”]*$")
                .WithMessage("{PropertyName} must not contain the following characters £ # “”.");

            RuleFor(workspace => workspace.Description)
                .NotNull()
                .Length(0, 200)
                .Matches("^[^£#“”]*$")
                .WithMessage("{PropertyName} must not contain the following characters £ # “”.");
        }
    }
}
