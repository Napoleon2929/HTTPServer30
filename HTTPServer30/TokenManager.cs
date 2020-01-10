using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeLibrary;
using HTTPServer30.Models;

namespace HTTPServer30
{
    public static class TokenManager
    {
        private static OfficeDBEntities officeDB;
        static TokenManager()
        {
            officeDB = new OfficeDBEntities();
        }
        public static Tokens NewToken(string login, int hours)
        {
            var time = DateTime.UtcNow;
            return new Tokens
            {
                Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                Employee_Login = login,
                Creation_Time = time,
                End_time = time.AddHours(hours)
            };
        }
        public static bool ValidateToken(Tokens token)
        {
            var time = DateTime.UtcNow;
            if (token == null)
                return false;
            return token.End_time > time && time > token.Creation_Time;
        }
        public static void CleanInvalideTokens(string login)
        {
            var oldTokens = officeDB.Tokens.Where(t => t.Employee_Login == login);
            foreach (var token in oldTokens)
            {
                if (!ValidateToken(token))
                    officeDB.Tokens.Remove(token);
            }
            officeDB.SaveChangesAsync();
        }
        public static List<TokensToView> GetValideTokens(string login)
        {
            var tokens = officeDB.Tokens.Where(t => t.Employee_Login == login);
            List<TokensToView> items = new List<TokensToView>();
            foreach (var token in tokens)
            {
                if (!TokenManager.ValidateToken(token))
                    officeDB.Tokens.Remove(token);
                else
                    items.Add(new TokensToView(token));
            }
            officeDB.SaveChanges();
            return items;
        }
        public static void CleanAllTokens(string login)
        {
            var tokens = officeDB.Tokens.Where(t => t.Employee_Login == login);
            officeDB.Tokens.RemoveRange(tokens);
            officeDB.SaveChangesAsync();
        }
        public static Status GetStatus(string login)
        {
            var tokens = officeDB.Tokens.Where(t => t.Employee_Login == login);
            if (tokens == null || tokens.Count() == 0)
                return Status.Offline;
            var result = Status.Indefined;
            foreach (var token in tokens)
            {
                result = Status.Offline;
                if (ValidateToken(token))
                {
                    result = Status.Online;
                    break;
                }
            }
            return result;
        }
    }
}