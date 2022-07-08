using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity.Categories
{
    [Table("categoria", Schema = "dbo")]
    public class Category
    {
        [Key, Column("id_categoria")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("ds_categoria")]
        public string Name { get; set; }

        [Column("id_empresa")]
        public int EnterpriseId { get; set; }

        [Column("status")]
        public bool Status { get; set; } = true;

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

        [Column("dt_alteracao")]
        public DateTime? AlterDate { get; set; }
    }
}
