using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Fidelities.Models
{
    public class FidelityViewModel
    {
        public int Id { get; set; }
        public int LoyaltId { get; set; }
        public int ConsumedProductId { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime AlterDate { get; set; } = DateTime.UtcNow;
    }
}