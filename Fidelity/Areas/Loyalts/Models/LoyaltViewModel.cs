using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Loyalts.Models
{
    public class LoyaltViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int EnterpriseId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Limit { get; set; }
        public int ProductId { get; set; }
        public int PromotionType { get; set; }
        public int FidelityId { get; set; }
        public double Quantity { get; set; }
    }
}