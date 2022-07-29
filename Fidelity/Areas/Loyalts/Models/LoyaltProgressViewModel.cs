using Fidelity.Areas.Clients.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Loyalts.Models
{
    public class LoyaltProgressViewModel
    {
        public ClientViewModel Client { get; set; }
        public List<LoyaltProgressModel> LoyaltProgress { get; set; }
    }
}