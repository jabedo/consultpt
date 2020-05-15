﻿using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app
{
    /// <summary>
    /// This authorisation handler will bypass all requirements
    /// </summary>
    public class AllowAnonymous : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (IAuthorizationRequirement requirement in context.PendingRequirements.ToList())
                context.Succeed(requirement); //Simply pass all requirements

            return Task.CompletedTask;
        }
    }
}
