using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.BusinessLogic.Hubs
{
    [Authorize]
    public class NotificationHub : CommonHub
    {
        public NotificationHub(IRepository<User> usersRepo) : base(usersRepo)
        {

        }
    }
}
