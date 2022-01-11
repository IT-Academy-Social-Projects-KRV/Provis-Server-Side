using FluentValidation;
using Provis.Core.DTO.UserDTO;
using Provis.Core.DTO.TaskDTO;
using Provis.Core.Statuses;
using System;

namespace Provis.Core.Validation
{
    public class CreateTaskValidation : AbstractValidator<TaskCreateDTO>
    {
        public CreateTaskValidation()
        {
            RuleFor(task => task.Name)
                .NotEmpty()
                .NotNull();

            RuleFor(task => task.DateOfEnd)
                .NotEmpty()
                .NotNull()
                .GreaterThan(DateTime.UtcNow);

            RuleFor(task => task.WorkspaceId)
                .NotEmpty()
                .NotNull();
            RuleFor(task => task.StatusId)
                .NotEmpty()
                .NotEmpty()
                .Must(IsStatusExist)
                .WithMessage("This status not exist");
        }
        public bool IsStatusExist(int StatusId)
        {
            return Enum.IsDefined(typeof(TaskStatuses), StatusId);
        }
    }
}
