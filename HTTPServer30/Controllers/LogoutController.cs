using HTTPServer30.Filters;
using OfficeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;

namespace HTTPServer30.Controllers
{

    public class LogoutController : ApiController
    {
        private OfficeDBEntities officeDB = new OfficeDBEntities();
        [TokenAuthentication, HttpDelete]
        public void CloseSession()
        {
            string contextToken = Request.Headers.Authorization.Parameter;
            officeDB.Tokens.Remove(officeDB.Tokens.FirstOrDefault(t => t.Token == contextToken));
            officeDB.SaveChangesAsync();
        }
    }
}
