using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity.Checkpoints
{
    [Table("checkpoint", Schema = "dbo")]
    public class Checkpoint
    {
        [Key, Column("id_checkpoint")]
        public int? Id { get; set; }

        [Column("id_cliente")]
        public int ClientId { get; set; }

        [Column("id_fidelidade")]
        public int LoyaltId { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;
    }
}
