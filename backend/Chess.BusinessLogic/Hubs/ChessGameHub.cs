using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.BusinessLogic.Hubs
{
    [Authorize]
    public class ChessGameHub : CommonHub
    {
    }
}
