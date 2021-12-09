using Provis.Core.Interfaces;
using System;

namespace Provis.Core.Entities
{
    public class StatusHistory : IBaseEntity
    {
        public int Id { get; set; }

        public DateTime DateOfChange { get; set; }

        public int TaskId { get; set; }

        public Task Task { get; set; }

        public int StatusId { get; set; }

        public Status Status { get; set; }
    }
}
