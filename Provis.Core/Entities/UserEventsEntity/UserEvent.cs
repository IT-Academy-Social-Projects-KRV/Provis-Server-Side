using Provis.Core.Entities.EventEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Interfaces;

namespace Provis.Core.Entities.UserEventsEntity
{
    public class UserEvent : IBaseEntity
    {
        public int EventId { get; set; }
        public Event Event { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
