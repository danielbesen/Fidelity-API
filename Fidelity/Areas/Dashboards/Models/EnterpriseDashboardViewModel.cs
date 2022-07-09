using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Dashboards.Models
{
    public class EnterpriseDashboardViewModel
    {
        public List<MostUsedLoyaltsViewModel> MostUsedLoyalts { get; set; }
        public int TotalClients { get; set; }

    }
}
