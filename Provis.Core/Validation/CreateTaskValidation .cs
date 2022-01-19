using FluentValidation;
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
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Due date should be not in the past");

            RuleFor(task => task.WorkspaceId)
                .NotEmpty()
                .NotNull();

            RuleFor(task => task.StatusId)
                .NotEmpty()
                .NotEmpty()
                .Must(IsStatusExist)
                .WithMessage("This status not exist");

            RuleFor(task => task.StoryPoints)
                .InclusiveBetween(1, 99)
                .WithMessage("Story Points most be greater than 0 and less 100");
        }
        public bool IsStatusExist(int StatusId)
        {
            return Enum.IsDefined(typeof(TaskStatuses), StatusId);
        }
    }
}
