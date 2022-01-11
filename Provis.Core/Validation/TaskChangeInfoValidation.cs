using FluentValidation;
using Provis.Core.DTO.TaskDTO;
using System;

namespace Provis.Core.Validation
{
    public class TaskChangeInfoValidation : AbstractValidator<TaskChangeInfoDTO>
    {
        public TaskChangeInfoValidation()
        {
            RuleFor(task => task.Name)
                .NotEmpty()
                .NotNull();

            RuleFor(task => task.Description)
                .NotEmpty()
                .NotNull();

            RuleFor(task => task.Deadline)
                .NotEmpty()
                .NotNull()
                .GreaterThan(DateTime.UtcNow);
        }
    }
}
