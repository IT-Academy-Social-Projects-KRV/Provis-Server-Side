using Provis.Core.Interfaces;

namespace Provis.Core.Entities
{
    public class UsersTasks : IBaseEntity
    {
        public string UserId { get; set; }

        public User User { get; set; }

        public int TaskId { get; set; }

        public Task Task { get; set; }

        public bool IsDeleted { get; set; }
    }
}
