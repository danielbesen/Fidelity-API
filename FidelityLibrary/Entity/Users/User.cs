using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity.Users
{
    [Table("public.usuario", Schema = "public")]
    public class User
    {
        [Key, Column("id_usuario")]
        public int Id { get; set; }

        [Column("nome_usuario")]
        [Required(ErrorMessage = "O Nome de usuário é obrigatório", AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Column("email")]
        [Required(ErrorMessage = "O Email do usuário é obrigatório", AllowEmptyStrings = false)]
        public string Email { get; set; } 

        [Column("tipo_usuario")]
        public string Type { get; set; }

        [Column("img_usuario")]
        public byte[] Image { get; set; }

        [Column("ativo")]
        public string Active { get; set; }

        [Column("senha_usuario")]
        [Required(ErrorMessage = "A senha do usuário é obrigatória", AllowEmptyStrings = false)]
        public string Password { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

        [Column("dt_alteracao")]
        public DateTime AlterDate { get; set; }
    }
}
