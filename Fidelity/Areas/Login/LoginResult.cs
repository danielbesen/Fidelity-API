using Fidelity.Areas.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Login
{
    public class LoginResult<T>
    {
        public string Type { get; set; }

        public T Property { get; set; }

        public byte[] Image { get; set; }
        public Object Token { get; set; }
    }
}