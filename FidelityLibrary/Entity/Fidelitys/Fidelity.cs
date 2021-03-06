using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity.Fidelitys
{
    [Table("fidelizacao", Schema = "dbo")]
    public class Fidelity
    {
        [Key, Column("id_fidelizacao")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("id_fidelidade")]
        public int LoyaltId { get; set; }

        [Column("id_produto")]
        public int ConsumedProductId { get; set; }

        [Column("status")]
        public bool Status { get; set; } = true;
        
        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

        [Column("dt_alteracao")]
        public DateTime? AlterDate { get; set; }
    }
}