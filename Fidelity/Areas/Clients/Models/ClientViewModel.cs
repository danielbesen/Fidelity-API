using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Clients.Models
{
    public class ClientViewModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Cpf { get; set; }
    }
}