using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Employes.Models
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EnterpriseId { get; set; }
        public int AccessType { get; set; }
        public DateTime AlterDate { get; set; }
        public byte[] Image { get; set; }
    }
}