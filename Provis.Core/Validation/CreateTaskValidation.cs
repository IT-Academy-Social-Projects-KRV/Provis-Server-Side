using FluentValidation;
using Provis.Core.DTO.workspaceDTO;

namespace Provis.Core.Validation
{
    public class CreateTaskValidation : AbstractValidator<TaskCreateDTO>
    {
        public CreateTaskValidation()
        {
            RuleFor(task => task.Name)
                .NotEmpty()
                .NotNull();

            RuleFor(task => task.Description)
                .NotEmpty()
                .NotNull();

            RuleFor(task => task.DateOfEnd)
                .NotEmpty()
                .NotNull();

            RuleFor(task => task.StatusID)
                .NotEmpty()
                .NotNull();

            RuleFor(task => task.WorkspaceID)
                .NotEmpty()
                .NotNull();
        }
    }
}
