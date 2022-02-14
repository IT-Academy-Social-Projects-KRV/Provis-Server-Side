﻿using System;

namespace Provis.Core.DTO.UserDTO
{
    public class UserRegistrationDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTimeOffset? BirthDay { get; set; }
    }
}
