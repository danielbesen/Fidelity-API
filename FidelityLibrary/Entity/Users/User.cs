using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity.Users
{
    [Table("usuario", Schema = "dbo")]
    public class User
    {
        [Key, Column("id_usuario")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("email")]
        [Required(ErrorMessage = "O Email do usuário é obrigatório", AllowEmptyStrings = false)]
        public string Email { get; set; } 

        [Column("tipo_usuario")]
        public string Type { get; set; }

        [Column("img_usuario")]
        public byte[] Image { get; set; }

        [Column("status")]
        public bool Status { get; set; } = true;

        [Column("senha_usuario")]
        [Required(ErrorMessage = "A senha do usuário é obrigatória", AllowEmptyStrings = false)]
        public string Password { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

        [Column("dt_alteracao")]
        public DateTime? AlterDate { get; set; }
    }
}
