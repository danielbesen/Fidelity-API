using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity.Products
{
    [Table("produto", Schema = "dbo")]
    public class Product
    {
        [Key, Column("id_produto")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        [Column("ds_produto")]
        public string Description { get; set; }

        [Column("id_empresa")]
        public int EnterpriseId { get; set; }

        [Column("valor")]
        public double Value { get; set; }

        [Column("id_categoria")]
        public int? CategoryId { get; set; }

        [Column("status")]
        public bool Status { get; set; } = true;

        [Column("img_produto")]
        public byte[] Image { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

        [Column("dt_alteracao")]
        public DateTime? AlterDate { get; set; }
    }
}