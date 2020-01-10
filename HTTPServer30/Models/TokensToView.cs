using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTTPServer30.Models
{
    public class TokensToView
    {
        public string Token { get; set; }
        public DateTime? Creation_Time { get; set; }
        public DateTime? End_time { get; set; }

        public TokensToView(OfficeLibrary.Tokens tokens)
        {
            Token = tokens.Token;
            Creation_Time = tokens.Creation_Time;
            End_time = tokens.End_time;
        }
    }
}