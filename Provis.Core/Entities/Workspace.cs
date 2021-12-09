using Provis.Core.Interfaces;
using System;

namespace Provis.Core.Entities
{
    public class Workspace : IBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime DateOfCreate { get; set; }
    }
}
