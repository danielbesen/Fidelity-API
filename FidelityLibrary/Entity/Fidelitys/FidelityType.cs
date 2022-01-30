using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity.Fidelitys
{
    [Table("tipo_fidelizacao", Schema = "public")]
    public class FidelityType
    {
        [Key, Column("id_tipo_fidelizacao") ]
        public int? Id { get; set; }

        [Column("ds_tipo_fidelizacao") ]
        public int Name { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

        [Column("dt_alteracao")]
        public DateTime AlterDate { get; set; }
    }
}
