using FluentValidation;
using Provis.Core.DTO.TaskDTO;
using Provis.Core.Resources;
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
                .WithMessage(ErrorMessages.InvalidDateOfEnd);

            RuleFor(task => task.WorkspaceId)
                .NotEmpty()
                .NotNull();

            RuleFor(task => task.StatusId)
                .NotNull()
                .NotEmpty()
                .Must(IsStatusExist)
                .WithMessage(ErrorMessages.TaskStatusNotFound);

            RuleFor(task => task.StoryPoints)
                .InclusiveBetween(1, 99)
                .WithMessage(ErrorMessages.InvalidStoryPoints);
        }
        public bool IsStatusExist(int StatusId)
        {
            return Enum.IsDefined(typeof(TaskStatuses), StatusId);
        }
    }
}
