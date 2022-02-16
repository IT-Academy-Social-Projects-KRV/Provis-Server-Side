using FluentValidation;
using Provis.Core.DTO.CalendarDTO;

namespace Provis.Core.Validation
{
    class EventDayValidation : AbstractValidator<EventDayDTO>
    {
        public EventDayValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .Length(1, 50);
        }
    }
}
