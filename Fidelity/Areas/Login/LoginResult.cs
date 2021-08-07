using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Login
{
    public class LoginResult<T>
    {
        public char Type { get; set; }

        public Object Token { get; set; }

        public T Property { get; set; }
    }
}