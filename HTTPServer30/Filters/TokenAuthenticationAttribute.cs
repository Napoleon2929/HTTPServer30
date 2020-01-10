using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Security.Principal;
using OfficeLibrary;
using System.Web.Http.Results;

namespace HTTPServer30.Filters
{
    public class TokenAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        private int? accessLevel = null;
        private OfficeDBEntities officeDB = new OfficeDBEntities();
        public TokenAuthenticationAttribute() { }
        public TokenAuthenticationAttribute(int level)
        {
            accessLevel = level;
        }
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            context.Principal = null;
            int? contextlevel = null;
            AuthenticationHeaderValue authentication = context.Request.Headers.Authorization;
            if (authentication != null && authentication.Scheme == "Bearer")
            {
                //OfficeDBEntities officeDB = new OfficeDBEntities();
                var token = officeDB.Tokens.FirstOrDefault(t => t.Token == authentication.Parameter);
                if (token != null)
                {
                    contextlevel = officeDB.Employees.FirstOrDefault(e => e.Login == token.Employee_Login).Security;
                    if (TokenManager.ValidateToken(token))
                    {
                        if (accessLevel != null && contextlevel >= accessLevel)
                            context.Principal = new GenericPrincipal(new GenericIdentity(token.Employee_Login), null);
                        else if (accessLevel == null)
                            context.Principal = new GenericPrincipal(new GenericIdentity(token.Employee_Login), null);
                    }
                }
            }
            if (context.Principal == null)
            {
                if (accessLevel != null && contextlevel < accessLevel)
                    context.ErrorResult = new StatusCodeResult(System.Net.HttpStatusCode.MethodNotAllowed, context.Request);
                else
                    context.ErrorResult = new StatusCodeResult(System.Net.HttpStatusCode.Unauthorized, context.Request);
            }
            return Task.FromResult<object>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(null);
        }
        public bool AllowMultiple
        {
            get => false;
        }
    }
}