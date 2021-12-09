using Provis.Core.Interfaces;

namespace Provis.Core.Entities
{
    public class Status : IBaseEntity
    {
        public int Id { get; set; }

        public string StatusName { get; set; }
    }
}
