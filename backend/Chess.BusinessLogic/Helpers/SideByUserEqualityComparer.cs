using Chess.DataAccess.Entities;
using System.Collections.Generic;

namespace Chess.BusinessLogic.Helpers
{
    public class SideByUserEqualityComparer : IEqualityComparer<Side>
    {
        public bool Equals(Side x, Side y)
        {
            return x?.PlayerId == y?.PlayerId;
        }

        public int GetHashCode(Side obj)
        {
            return obj.GetHashCode();
        }
    }
}
