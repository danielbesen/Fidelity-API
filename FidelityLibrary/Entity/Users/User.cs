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

        [Column("email")]
        [Required(ErrorMessage = "O Email do usuário é obrigatório", AllowEmptyStrings = false)]
        public string Email { get; set; }

        [Column("tipo_usuario")]
        public string Type { get; set; }

        [Column("senha_usuario")]
        [Required(ErrorMessage = "A senha do usuário é obrigatória", AllowEmptyStrings = false)]
        public string Password { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

    }
}
