using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity
{
    [Table("funcionario", Schema = "public")]
    public class Employee
    {
        [Key, Column("id_func")]
        public int Id { get; set; }

        [Column("id_usuario")]
        public int UserId { get; set; }

        [Column("id_empresa")]
        public int EnterpriseId { get; set; }

        [Column("img_func")]
        public byte[] Image { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("id_status")]
        public char Status { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;
    }
}
