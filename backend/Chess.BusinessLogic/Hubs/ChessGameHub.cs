using Chess.Common.Interfaces;
using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.BusinessLogic.Hubs
{
    [Authorize]
    public class ChessGameHub : CommonHub
    {
        public ChessGameHub(IRepository<User> usersRepo) 
            : base(usersRepo)
        {

        }
    }
}
