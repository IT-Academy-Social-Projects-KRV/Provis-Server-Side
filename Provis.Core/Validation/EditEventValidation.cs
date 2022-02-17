using FluentValidation;
using Provis.Core.DTO.CalendarDTO;

namespace Provis.Core.Validation
{
    class EditEventValidation : AbstractValidator<EventEditDTO>
    {
        public EditEventValidation()
        {
            RuleFor(x => x.EventMessage)
                .Length(1, 1000);

            RuleFor(x => x.EventName)
                .NotEmpty()
                .NotNull()
                .Length(1, 50);
        }
    }
}