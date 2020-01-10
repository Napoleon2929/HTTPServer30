using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HTTPServer30.Filters;
using HTTPServer30.Models;
using OfficeLibrary; 

namespace HTTPServer30.Controllers
{
    [TokenAuthentication]
    public class ProfileController : ApiController
    {
        private OfficeDBEntities officeDB = new OfficeDBEntities();
        
        public EmployeesToView GetMyProfile()
        {
            string contextLogin = RequestContext.Principal.Identity.Name;
            return new EmployeesToView(officeDB.Employees.FirstOrDefault(t => t.Login == contextLogin), Status.Online);
        }
        
        public EmployeesToView GetByLogin(string login)
        {
            return new EmployeesToView(officeDB.Employees.FirstOrDefault(t => t.Login == login), TokenManager.GetStatus(login));
        }
        
        public IEnumerable<EmployeesToView> GetByString(string search)
        {
            var words = search.Split(' ');
            List<EmployeesToView> views = new List<EmployeesToView>();
            //IQueryable<Employees> list = null;
            switch (words.Length) 
            {
                case 1:
                    var byLogin = GetByLogin(words[0]);
                    if (byLogin != null && byLogin.Login!=null)
                        views.Add(byLogin);
                    var list = officeDB.Employees.Where(t => t.Name == search || t.Surname == search).ToList();
                    foreach (var employees in list)
                        views.Add(new EmployeesToView(employees, TokenManager.GetStatus(employees.Login)));
                    break;
                case 2:
                    //don't work without this 
                    string word0 = words[0];
                    string word1 = words[1]; 
                    //should look like that better result of search on the top
                    list = officeDB.Employees.Where(t =>
                    (t.Name == word1 && t.Surname == word0) || (t.Name == word0 && t.Surname == word1)).ToList();
                    foreach (var employees in list)
                        views.Add(new EmployeesToView(employees, TokenManager.GetStatus(employees.Login)));
                    if (views.Count == 0)
                    {
                        list = null;
                        list = officeDB.Employees.Where(t =>
                            t.Name == word0 || t.Surname == word0 || t.Name == word1 || t.Surname == word1).ToList();
                        foreach (var employees in list)
                        {
                            bool canAdd = true;
                            for (var i = 0; i < views.Count; i++)
                            {
                                if (employees.Login == views[i].Login)
                                {
                                    canAdd = false;
                                    break;
                                }
                            }
                            if (canAdd)
                                views.Add(new EmployeesToView(employees, TokenManager.GetStatus(employees.Login)));
                        }
                    }
                    break;
            }
            return views;
        }
    }
}
