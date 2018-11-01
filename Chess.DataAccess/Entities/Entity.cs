using System;
namespace Chess.DataAccess.Entities
{
    public class Entity : IEquatable<Entity>
    {
        public int Id { get; set; }

        public bool Equals(Entity other)
        {
            return other?.Id == Id;
        }
    }
}
