using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Enterprises.Models
{
    public class EnterpriseViewModel
    {
        public string Name { get; set; }

        public string Cnpj { get; set; }

        public string Tel { get; set; }

        public string Address { get; set; }

        public string AddressNum { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Branch { get; set; }

        public int? MembershipId { get; set; }

        public bool Active { get; set; }

        public DateTime AlterDate { get; set; }
    }
}