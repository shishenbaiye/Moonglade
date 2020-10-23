﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Moonglade.Model
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime LastLoginTimeUtc { get; set; }
        public string LastLoginIp { get; set; }
        public DateTime CreateOnUtc { get; set; }
    }
}
