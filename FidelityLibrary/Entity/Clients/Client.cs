﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity
{
    [Table("cliente", Schema = "public")]
    public class Client 
    {
        [Key, Column("id_cliente")]
        public int? Id { get; set; }

        [Column("id_usuario")]
        public int? UserId { get; set; }

        [Column("nome")]
        [Required(ErrorMessage = "O nome do usuário é obrigatório", AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Column("cpf_cliente")]
        [Required(ErrorMessage = "O cpf é obrigatório", AllowEmptyStrings = false)]
        public string Cpf { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;
    }
}
