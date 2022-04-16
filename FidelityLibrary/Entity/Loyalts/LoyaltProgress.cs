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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Column("id_cliente")]
        public int ClientId { get; set; }

        [Column("nr_pontos")]
        public double Points { get; set; }

        [Column("status")]
        public bool Status { get; set; }

        [Column("id_fidelidade")]
        public int LoyaltId { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;
    }
}
