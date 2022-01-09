using Fidelity.Areas.Clients.Models;
using Fidelity.Areas.Enterprises.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Users.Models
{
    public class UserViewModel
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Type { get; set; }

        public string Active { get; set; }

        public byte[] Ímage { get; set; }

        public ClientViewModel Client { get; set; }

        public EnterpriseViewModel Enterprise { get; set; }
    }
}