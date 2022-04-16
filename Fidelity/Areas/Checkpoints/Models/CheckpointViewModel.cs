using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Checkpoints.Models
{
    public class CheckpointViewModel
    {
        public int  Id { get; set; }
        public int ClientId { get; set; }
        public int LoyaltId { get; set; }
        public double Value { get; set; }
    }
}