using Provis.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Provis.Core.Entities
{
    public class Status : IBaseEntity
    {
        [Key]
        public int Id { get; set; }

        public string StatusName { get; set; }
    }
}
