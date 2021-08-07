using Fidelity_Library.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fidelity_Library.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("PgConnection")
        {

        }

        public virtual DbSet<Client> DbSetClient { get; set; }
    }
}