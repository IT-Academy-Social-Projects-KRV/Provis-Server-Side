using Provis.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Provis.Core.Entities
{
    public class UserRoleTag : IBaseEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
