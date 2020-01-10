using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTTPServer30.Models
{
    public class EmployeesToView
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? Birthday { get; set; }
        public int? Security { get; set; }
        public string Position { get; set; }
        public DateTime? Hiring_Time { get; set; }
        public Status Status{ get; set; }
        public EmployeesToView(OfficeLibrary.Employees employees)
        {
            if (employees == null)
            {
                return;
            }   
            Login = employees.Login;
            Name = employees.Name;
            Surname = employees.Surname;
            Birthday = employees.Birthday;
            Security = employees.Security;
            Position = employees.Position;
            Hiring_Time = employees.Hiring_Time;
            Status = Status.Indefined;
        }
        public EmployeesToView(OfficeLibrary.Employees employees, Status status)
            :this(employees)
        {
            Status = status;
        }
        public EmployeesToView() { }
    }
}