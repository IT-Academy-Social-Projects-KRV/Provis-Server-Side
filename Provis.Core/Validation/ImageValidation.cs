using FluentValidation;
using Microsoft.Extensions.Options;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Helpers;
using Provis.Core.Resources;
using System;
using System.Linq;

namespace Provis.Core.Validation
{
    public class ImageValidation: AbstractValidator<UserUploadImageDTO>
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
                .WithMessage(ErrorMessages.InvalidImage);
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
