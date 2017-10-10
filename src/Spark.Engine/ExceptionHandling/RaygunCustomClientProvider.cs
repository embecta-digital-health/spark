using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Messages;

namespace Spark.Engine.ExceptionHandling
{
    public class RaygunCustomClientProvider : DefaultRaygunAspNetCoreClientProvider
    {
        private const string RealmAccess = "realm_access";

        public override RaygunClient GetClient(RaygunSettings settings, HttpContext context)
        {
            RaygunClient client = base.GetClient(settings, context);

            var identity = context?.User?.Identity as ClaimsIdentity;
            if (identity?.IsAuthenticated == true)
            {
//                string email = identity.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).FirstOrDefault();
                string[] roles = identity.Claims.Where(c => c.Type == RealmAccess).Select(c => c.Value).ToArray();

                client.UserInfo = new RaygunIdentifierMessage(identity.Name)
                {
                    IsAnonymous = false,
                    FullName = identity.Name,
                    Identifier = string.Join(", ",roles)
                };
            }

            return client;
        }
    }
}