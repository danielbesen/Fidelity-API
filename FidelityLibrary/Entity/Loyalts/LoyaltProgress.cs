using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity.Loyalts
{
    [Table("progresso_fidelidade", Schema = "dbo")]
    public class LoyaltProgress
    {
        [Key, Column("id_progresso_fidelidade")]
        public int? Id { get; set; }

        [Column("id_cliente")]
        public int ClientId { get; set; }

        [Column("id_checkpoint")]
        public int? id_checkpoint { get; set; }

        [Column("nr_pontos")]
        public int Points { get; set; }

        [Column("status")]
        public bool Status { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;
    }
}
