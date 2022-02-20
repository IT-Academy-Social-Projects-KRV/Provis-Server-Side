using FluentValidation;
using Provis.Core.DTO.WorkspaceDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provis.Core.Validation
{
    public class SprintSettingsValidation: AbstractValidator<SprintSettingsDTO>
    {
        public SprintSettingsValidation()
        {
            RuleFor(x => x.IsUseSprints)
                .NotNull();
        }
    }
}
