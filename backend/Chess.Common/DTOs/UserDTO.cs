﻿using System;

namespace Chess.Common.DTOs
{
    public class UserDTO: DbEntityDTO
    {
        public string Name { get; set; }
        public string Uid { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastSeenDate { get; set; }
        public bool IsOnline { get; set; }
    }
}
