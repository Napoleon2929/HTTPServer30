using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HTTPServer30.Filters;
using OfficeLibrary;

namespace HTTPServer30.Controllers
{
    [TokenAuthentication]
    public class MessageController : ApiController
    {
        private OfficeDBEntities officeDB = new OfficeDBEntities();
        [HttpPost]
        public void NewMessage([FromBody]string addresat, [FromBody]string text)
        {
            string contextLogin = RequestContext.Principal.Identity.Name;
            officeDB.Messages.Add(new Messages
            {
                Sender = contextLogin,
                Adressat = addresat,
                Text = text,
                Sending_Time = DateTime.Now,
                IsRead = false,
                IsGet = false
            });
            officeDB.SaveChangesAsync();
        }
        public IEnumerable<Messages> GetDialog(string interlocutor)
        {
            string contextLogin = RequestContext.Principal.Identity.Name;
            var messages = officeDB.Messages.Where(t => t.Sender == interlocutor && t.Adressat == contextLogin).ToList();
            messages.AddRange(officeDB.Messages.Where(t => t.Sender == contextLogin && t.Adressat == interlocutor).ToList());
            messages.Sort((x, y) => DateTime.Compare(x.Sending_Time, y.Sending_Time));
            return messages;
        }
        public IEnumerable<Messages> GetNewMessages()
        {
            string contextLogin = RequestContext.Principal.Identity.Name;
            var messages = officeDB.Messages.Where(t => t.IsGet == false && t.Adressat == contextLogin).ToList();
            messages.Sort((x, y) => DateTime.Compare(x.Sending_Time, y.Sending_Time));
            return messages;
        }

        [HttpPost]
        public void MarkRecievedMessage([FromBody]int id)
        {
            var message = officeDB.Messages.Where(t => t.Id == id).FirstOrDefault();
            message.IsGet = true;
            officeDB.SaveChangesAsync();
        }
    }
}
