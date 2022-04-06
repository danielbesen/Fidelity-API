using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Loyalts.Models
{
    public class LoyaltProgressViewModel
    {
        public int? Id { get; set; }
        public int ClientId { get; set; }
        public int id_checkpoint { get; set; }
        public int Points { get; set; }
        public bool Status { get; set; }
    }
}