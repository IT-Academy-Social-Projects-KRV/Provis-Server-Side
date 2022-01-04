﻿using Provis.Core.DTO.userDTO;
using System;
using System.Collections.Generic;

namespace Provis.Core.DTO.workspaceDTO
{
    public class TaskCreateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateOfEnd { get; set; }
        public int WorkspaceId { get; set; }
        public int StatusId { get; set; }
        public List<AssignedUserOnTaskDTO>  AssignedUsers { get; set; }
    }
}