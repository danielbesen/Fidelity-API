using Fidelity.Areas.Loyalts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Enterprises.Models
{
    public class EnterpriseViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Cnpj { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public string AddressNum { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Branch { get; set; }
        public int? MembershipId { get; set; }
        public bool Status { get; set; }
        public DateTime? AlterDate { get; set; }
        public List<LoyaltViewModel> Loyalts { get; set; }
    }
}