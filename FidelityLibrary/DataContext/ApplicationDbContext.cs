using FidelityLibrary.Entity;
using FidelityLibrary.Entity.Users;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("PgConnection")
        {

        }

        public virtual DbSet<Client> DbSetClient { get; set; }
        public virtual DbSet<User> DbSetUser { get; set; }
        public virtual DbSet<Enterprise> DbSetEnterprise { get; set; }
        public virtual DbSet<Employee> DbSetEmployee { get; set; }
    }
}