using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity.Memberships
{
    [Table("plano_app", Schema = "dbo")]
    public class Membership
    {

        [Key, Column("id_plano")]
        public int? Id { get; set; }

        [Column("ds_plano")]
        public string Name { get; set; }

        [Column("valor")]
        public double Value { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

        [Column("dt_alteracao")]
        public DateTime AlterDate { get; set; }

    }
}
