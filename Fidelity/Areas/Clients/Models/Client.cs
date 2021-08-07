using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Fidelity.Areas.Clients.Models
{
    [Table("cliente", Schema = "public")]
    public class Client
    {
        [Key, Column("id_cliente")]
        public int Id { get; set; }

        [Column("id_usuario")]
        public int UserId { get; set; }

        [Column("nome")]
        public string Name { get; set; }

        [Column("cpf_cliente")]
        public string Cpf { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;
    }
}