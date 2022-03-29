using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Clients.Models
{
    public class ClientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Cpf { get; set; }
    }
}