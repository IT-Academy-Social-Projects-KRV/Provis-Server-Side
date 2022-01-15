using FluentValidation;
using Microsoft.Extensions.Options;
using Provis.Core.DTO.userDTO;
using Provis.Core.Helpers;
using System;
using System.Linq;

namespace Provis.Core.Validation
{
    public class ImageValidation: AbstractValidator<UploadImageDTO>
    {
        private readonly IOptions<ImageSettings> options;

        public ImageValidation(IOptions<ImageSettings> options)
        {
            this.options = options;

            RuleFor(x => x.Image)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Image.Length)
                .LessThanOrEqualTo(options.Value.MaxSize * 1024 * 1024)
                .WithMessage($"Max size is {options.Value.MaxSize} Mb");

            RuleFor(x => x.Image.ContentType)
               .Must(IsImage)
                .WithMessage("File must be image");
        }

        private bool IsImage(string contentType)
        {
            var type = contentType.Split("/");

            if(type[0] != options.Value.Type)
            {
                return false;
            }

            return options.Value.Subtypes.Contains(type[1]);
        }
    }
}
