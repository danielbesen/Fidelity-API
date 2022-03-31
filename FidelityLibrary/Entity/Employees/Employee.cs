using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity
{
    [Table("funcionario", Schema = "dbo")]
    public class Employee
    {
        [Key, Column("id_func")]
        public int Id { get; set; }

        [Column("id_usuario")]
        public int UserId { get; set; }

        [Column("id_empresa")]
        public int EnterpriseId { get; set; }

        [Column("nome_func")]
        public string Nome { get; set; }

        [Column("tipo_acesso")]
        public int AccessType { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

        [Column("dt_alteracao")]
        public DateTime? AlterDate { get; set; }
    }
}
