using FluentValidation;
using Provis.Core.DTO.WorkspaceDTO;

namespace Provis.Core.Validation
{
    public class WorkspaceUpdateValidation : AbstractValidator<WorkspaceUpdateDTO>
    {
        public WorkspaceUpdateValidation()
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
