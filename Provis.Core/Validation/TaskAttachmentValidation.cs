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

            RuleFor(x => x.Attachments)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Attachments)
                .Must(IsTaskAttachment)
                .WithMessage($"Files must not have one of these extenstion: " +
                $"{String.Join(", ", options.Value.SubtypesBlackList)}");

            RuleFor(x => x.Attachments)
                .Must(CheckSize)
                .WithMessage($"Max size is {options.Value.MaxSize} Mb");

        }

        private bool IsTaskAttachment(List<IFormFile> attachments)
        {
            bool isGood = true;

            foreach (var item in attachments)
            {
                var type = item.ContentType.Split("/");

                if (type[0] != options.Value.Type)
                {
                    isGood = false;
                    break;
                }

                if(!options.Value.SubtypesBlackList.Contains(type[1]))
                {
                    isGood = false;
                    break;
                }
            }

            return isGood;

        }

        private bool CheckSize(List<IFormFile> attachments)
        {
            bool isGood = true;

            foreach (var item in attachments)
            {
                if (item.Length >= options.Value.MaxSize * 1024 * 1024) 
                {
                    isGood = false;
                    break;
                }
            }

            return isGood;

        }
    }
}
