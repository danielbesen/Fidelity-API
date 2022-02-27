using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity.Promotions
{
    [Table("tipo_promocao", Schema = "dbo")]
    public class PromotionType
    {
        [Key, Column("id_tipo_promocao")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Column("ds_tipo_promocao")]
        public string Description { get; set; }

        [Column("titulo_promocao")]
        public string Title { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

        [Column("dt_alteracao")]
        public DateTime AlterDate { get; set; }
    }
}
