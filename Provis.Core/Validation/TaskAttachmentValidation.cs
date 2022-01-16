using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Provis.Core.DTO.TaskDTO;
using Provis.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Provis.Core.Validation
{
    public class TaskAttachmentValidation : AbstractValidator<TaskAttachmentsDTO>
    {
        private readonly IOptions<TaskAttachmentSettings> options;

        public TaskAttachmentValidation(IOptions<TaskAttachmentSettings> options)
        {
            this.options = options;

            RuleFor(x => x.TaskId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.WorkspaceId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Attachment)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Attachment)
                .Must(IsTaskAttachment)
                .WithMessage($"File must not have one of these extenstion: " +
                $"{String.Join(", ", options.Value.SubtypesBlackList)}");

            RuleFor(x => x.Attachment)
                .Must(CheckSize)
                .WithMessage($"Max size is {options.Value.MaxSize} Mb");

        }

        private bool IsTaskAttachment(IFormFile attachment)
        {            
            var type = attachment.ContentType.Split("/");

            return !(options.Value.SubtypesBlackList.Contains(type[1]));
        }

        private bool CheckSize(IFormFile attachment)
        {  
            return attachment.Length <= options.Value.MaxSize * 1024 * 1024;
        }
    }
}
