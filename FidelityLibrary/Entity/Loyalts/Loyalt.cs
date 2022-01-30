using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity.Loyalts
{
    [Table("fidelidade", Schema = "public")]
    public class Loyalt
    {
        [Key, Column("id_fidel")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("titulo_fidel")]
        public string Name { get; set; }

        [Column("ds_fidel")]
        public string Description { get; set; }

        [Column("id_empresa")]
        public int EnterpriseId { get; set; }

        [Column("dt_inicio")]
        public DateTime? StartDate { get; set; }

        [Column("dt_vencimento")]
        public DateTime? EndDate { get; set; }

        [Column("limite")]
        public int Limit { get; set; }

        [Column("id_produto")]
        public int ProductId { get; set; }

        [Column("id_fidelizacao")]
        public int? FidelityId { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

        [Column("dt_alteracao")]
        public DateTime AlterDate { get; set; }
    }
}
