using FluentValidation;
using Provis.Core.DTO.TaskDTO;
using System;

namespace Provis.Core.Validation
{
    public class TaskChangeInfoValidation : AbstractValidator<TaskChangeInfoDTO>
    {
        public TaskChangeInfoValidation()
        {

            RuleFor(task => task.Id)
                .NotNull();

            RuleFor(task => task.Name)
                .NotEmpty()
                .NotNull();

            RuleFor(task => task.WorkspaceId)
                .NotEmpty()
                .NotNull();

            RuleFor(task => task.Deadline)
                .NotEmpty()
                .NotNull()
                .GreaterThan(DateTime.UtcNow);

            RuleFor(task => task.StoryPoints)
                .InclusiveBetween(1, 99)
                .WithMessage("Story Points most be greater than 0 and less 100");
        }
    }
}
