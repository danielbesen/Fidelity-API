using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity.Users
{
    [Table("usuario", Schema = "public")]
    public class User
    {
        [Key, Column("id_usuario")]
        public int Id { get; set; }

        [Column("nome_usuario")]
        public string Name { get; set; }

        [Column("tipo_usuario")]
        public char Type { get; set; }

        [Column("senha_usuario")]
        public string Password { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

    }
}
