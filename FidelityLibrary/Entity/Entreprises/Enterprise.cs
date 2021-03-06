using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Entity
{
    [Table("empresa", Schema = "dbo")]
    public class Enterprise
    {
        [Key, Column("id_empresa")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("id_usuario")]
        public int UserId { get; set; }

        [Column("nome")]
        public string Name { get; set; }

        [Column("cnpj")]
        public string Cnpj { get; set; }

        [Column("telefone")]
        public string Tel { get; set; }

        [Column("endereco")]
        public string Address { get; set; }

        [Column("endereco_num")]
        public string AddressNum { get; set; }

        [Column("estado")]
        public string State { get; set; }

        [Column("cidade")]
        public string City { get; set; }

        [Column("ramo")]
        public string Branch { get; set; }

        [Column("id_plano")]
        public int? MembershipId { get; set; }

        [Column("dt_inclusao")]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

        [Column("dt_alteracao")]
        public DateTime? AlterDate { get; set; } 
    }
}
