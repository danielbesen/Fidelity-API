using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Fidelities.Models
{
    public class FidelityViewModel
    {
        public int Id { get; set; }
        public int PromotionTypeId { get; set; }
        public int? ConsumedProductId { get; set; }
        public int FidelityTypeId { get; set; }
        public double Quantity { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime AlterDate { get; set; } = DateTime.UtcNow;
    }
}