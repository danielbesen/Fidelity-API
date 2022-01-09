using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Products.Models
{
    public class ProductViewModel
    {
        public string ProductName { get; set; }

        public double Value { get; set; }

        public string Category { get; set; }

        public string status { get; set; }

        public byte[] Image { get; set; }
    }
}