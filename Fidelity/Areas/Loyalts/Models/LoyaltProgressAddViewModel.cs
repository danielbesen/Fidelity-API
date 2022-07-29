using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Loyalts.Models
{
    public class LoyaltProgressAddViewModel
    {
        public int? Id { get; set; }
        public int ClientId { get; set; }
        public int LoyaltId { get; set; }
        public double Points { get; set; }
        public bool Status { get; set; }
    }
}