using Fidelity.Areas.Clients.Models;
using Fidelity.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Fidelity.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext():base("PgConnection")
        {

        }

        public virtual DbSet<Client> DbSetClient { get; set; }
    }
}