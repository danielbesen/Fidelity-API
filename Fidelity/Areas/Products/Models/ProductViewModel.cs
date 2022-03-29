using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Products.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public int EnterpriseId { get; set; }
        public string Name { get; set; }

        public double Value { get; set; }

        public int CategoryId { get; set; }

        public string Status { get; set; }

        public byte[] Image { get; set; }

        public List<int> LoyaltList { get; set; }
    }
}