using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity.Checkpoints
{
    [Table("progresso_fidelidade_dashboard", Schema = "dbo")]
    public class CheckpointHistory
    {
        [Key, Column("id_progresso_fidelidade_dashboard")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Column("id_cliente")]
        public int ClientId { get; set; }

        [Column("id_fidelidade")]
        public int LoyaltId { get; set; }

        [Column("id_empresa")]
        public int EnterpriseId { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;
    }
}
