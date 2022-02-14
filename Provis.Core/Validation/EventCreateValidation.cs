using FluentValidation;
using Provis.Core.DTO.CalendarDTO;

namespace Provis.Core.Validation
{
    class EventCreateValidation : AbstractValidator<EventCreateDTO>
    {
        public EventCreateValidation()
        {
            RuleFor(x => x.EventName)
                .NotEmpty()
                .NotNull()
                .Length(1, 50);

            RuleFor(x => x.EventMessage)
                .Length(1, 1000);
        }
    }
}
