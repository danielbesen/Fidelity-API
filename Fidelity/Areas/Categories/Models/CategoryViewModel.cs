using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Categories.Models
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EnterpriseId { get; set; }
        public bool Status { get; set; } = true;
        public DateTime DataInclusao { get; set; }
        public DateTime? DataAlteracao{ get; set; }
    }
}