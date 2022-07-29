using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Loyalts.Models
{
    public class LoyaltProgressModel
    {
        public int? Id { get; set; }
        public double Points { get; set; }
        public bool Status { get; set; }
        public LoyaltViewModel Loyalt { get; set; }
    }
}