﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTTPServer30.Models
{
    public class ChangePassword
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}