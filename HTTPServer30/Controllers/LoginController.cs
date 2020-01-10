using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HTTPServer30.Models;
using HTTPServer30.Filters;
using OfficeLibrary;


namespace HTTPServer30.Controllers
{
    [AllowAnonymous]
    public class LoginController : ApiController
    {
        // GET api/values/5
        private OfficeDBEntities officeDB = new OfficeDBEntities();
        public TokensToView GetNewSession(string login, string password)
        {
            //OfficeDBEntities officeDB = new OfficeDBEntities();
            var user = officeDB.Employees.Where(e => e.Login == login && e.Password == password).FirstOrDefault();
            if (user != null)
            {
                TokenManager.CleanInvalideTokens(user.Login);
                Tokens token = TokenManager.NewToken(user.Login, 8);
                officeDB.Tokens.Add(token);
                officeDB.SaveChanges();
                return new TokensToView(token);
            }
            else
                return null;
        }

        public IEnumerable<TokensToView> GetCurrentSessions(string currentLogin, string password)
        {
            var user = officeDB.Employees.Where(e => e.Login == currentLogin && e.Password == password).FirstOrDefault();
            if (user != null)
            { 
                return TokenManager.GetValideTokens(user.Login);
            }
            else
                return null;
        }

        [HttpPost,TokenAuthentication(2)]
        public void Hiring([FromBody]EmployeesToView beginner)
        {
            var currentEmployee = officeDB.Employees.FirstOrDefault(t => t.Login == RequestContext.Principal.Identity.Name);
            if(currentEmployee.Security < beginner.Security)
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            officeDB.Employees.Add(new Employees
            {
                Login = beginner.Login,
                Password = "qwerty",
                Name = beginner.Name,
                Surname = beginner.Surname,
                Birthday = beginner.Birthday,
                Hiring_Time = DateTime.UtcNow.Date,
                Security = beginner.Security,
                Position = beginner.Position
            });
            officeDB.SaveChangesAsync();
        }

        [HttpPut, TokenAuthentication]
        public void ChangePassword([FromBody]ChangePassword change)
        {
            string contextLogin = RequestContext.Principal.Identity.Name;
            var employees = officeDB.Employees.FirstOrDefault(t => t.Login == contextLogin);
            if (employees.Password != change.OldPassword)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            else
            {
                employees.Password = change.NewPassword;
                officeDB.SaveChangesAsync();
            }   
        }

        [HttpPut, TokenAuthentication(2)]
        public void ChangePassword(string forChange, [FromBody]string newPassword)
        {
            var currentEmployee = officeDB.Employees.FirstOrDefault(t => t.Login == RequestContext.Principal.Identity.Name);
            var employees = officeDB.Employees.FirstOrDefault(t => t.Login == forChange);
            if (currentEmployee == employees || employees.Security >= currentEmployee.Security)
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            else
            {
                employees.Password = newPassword;
                TokenManager.CleanAllTokens(employees.Login);
                officeDB.SaveChangesAsync();
            }
        }

        [HttpPut, TokenAuthentication(2)]
        public void Promotion(string forPromotion, [FromBody]Promotion promotion)
        {
            var currentEmployee = officeDB.Employees.FirstOrDefault(t => t.Login == RequestContext.Principal.Identity.Name);
            var employees = officeDB.Employees.FirstOrDefault(t => t.Login == forPromotion);
            if (currentEmployee == employees || employees.Security >= currentEmployee.Security || promotion.Security > currentEmployee.Security)
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            else
            {
                employees.Position = promotion.Position;
                employees.Security = promotion.Security;
                officeDB.SaveChangesAsync();
            }
        }

        [HttpPut,TokenAuthentication(2)]
        public void EditEmployee(string forEdit, [FromBody]EmployeesToView toView)
        {
            var currentEmployee = officeDB.Employees.FirstOrDefault(t => t.Login == RequestContext.Principal.Identity.Name);
            var employees = officeDB.Employees.FirstOrDefault(t => t.Login == forEdit);
            if (currentEmployee == employees || employees.Security >= currentEmployee.Security || toView.Security > currentEmployee.Security)
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            else
            {
                employees.Name = toView.Name;
                employees.Surname = toView.Surname;
                employees.Birthday = toView.Birthday;
                employees.Position = toView.Position;
                employees.Security = toView.Security;
            }
        }

        [HttpDelete,TokenAuthentication(2)]
        public void Dismissal(string forDismissal)
        {
            var currentEmployee = officeDB.Employees.FirstOrDefault(t => t.Login == RequestContext.Principal.Identity.Name);
            var employees = officeDB.Employees.FirstOrDefault(t => t.Login == forDismissal);
            if (employees.Security >= currentEmployee.Security)
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            else
            {
                officeDB.Employees.Remove(employees);
                //TokenManager.CleanAllTokens(employees.Login);
                officeDB.SaveChangesAsync();
            }
        }
    }
}
