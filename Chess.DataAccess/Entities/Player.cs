using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.DataAccess.Entities
{
    public class Player : Entity
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
