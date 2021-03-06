﻿using Chess.Common.Helpers;
using Chess.Common.Interfaces;
using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChessWeb.Authentication
{
    public class CurrentUser : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IRepository<User> _userProvider;

        private IDictionary<object, object> RequestLevelCache
        {
            get
            {
                var ctx = CurrentContext;
                if (ctx == null)
                {
                    throw new NotSupportedException("No request level cache: If unit testing, use InjectRequestLevelAndSessionCacheForTesting().");
                }
                return ctx.Items;
            }
        }

        public HttpContext CurrentContext => _accessor.HttpContext;

        public CurrentUser(IHttpContextAccessor accessor, IRepository<User> userRepository)
        {
            _accessor = accessor;
            _userProvider = userRepository;
        }
        public async Task<User> GetCurrentUserAsync()
        {
            User currentUser;
            if (RequestLevelCache["User"] == null)
            {
                currentUser = await CurrentUserContainerAsync();
                RequestLevelCache["User"] = currentUser;
                return currentUser;
            }
            var result = RequestLevelCache["User"] as User;
            return result;
        }

        public async Task<User> GetCurrentDbUserAsync()
        {
            User currentUser;
            if (RequestLevelCache["User"] == null || (currentUser = RequestLevelCache["User"] as User).Id < 1)
            {
                var userUid = CurrentContext.User.GetUid();
                currentUser = await _userProvider.GetOneAsync(u => string.Equals(u.Uid, userUid));
                RequestLevelCache["User"] = currentUser;
                return currentUser;
            }

            return currentUser;
        }

        public string GetCurrentUserUid()
        {
            return CurrentContext.User.GetUid();
        }

        private async Task<User> CurrentUserContainerAsync()
        {
            var userUid = CurrentContext.User.GetUid();
            var userName = CurrentContext.User.GetName();
            User currentUser = null;
            if (string.IsNullOrEmpty(userName))
            {
                currentUser = await _userProvider.GetOneAsync(u => string.Equals(u.Uid, userUid));
            }
            else
            {
                currentUser = new User()
                {
                    AvatarUrl = CurrentContext.User.GetProfilePicture(),
                    Name = userName,
                    Uid = userUid
                };
            }
            return currentUser;
        }
    }
}
