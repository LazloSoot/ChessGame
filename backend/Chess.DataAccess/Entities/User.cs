using System;

namespace Chess.DataAccess.Entities
{
    public class User : Entity
    {
        public string Name { get; set; }

        public string Uid { get; set; }

        public string AvatarUrl { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}
