using FidelityLibrary.Entity;
using FidelityLibrary.Entity.Categories;
using FidelityLibrary.Entity.Checkpoints;
using FidelityLibrary.Entity.Fidelitys;
using FidelityLibrary.Entity.Loyalts;
using FidelityLibrary.Entity.Memberships;
using FidelityLibrary.Entity.Products;
using FidelityLibrary.Entity.Promotions;
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
        public virtual DbSet<Product> DbSetProduct { get; set; }
        public virtual DbSet<Loyalt> DbSetLoyalt { get; set; }
        public virtual DbSet<Fidelity> DbSetFidelity { get; set; }
        public virtual DbSet<Membership> DbSetMembership { get; set; }
        public virtual DbSet<LoyaltProgress> DbSetLoyaltProgress { get; set; }
        public virtual DbSet<FidelityType> DbSetFidelityType { get; set; }
        public virtual DbSet<PromotionType> DbSetPromotionType { get; set; }
        public virtual DbSet<Category> DbSetCategory { get; set; }
        public virtual DbSet<Checkpoint> DbSetCheckpoint { get; set; }
        public virtual DbSet<CheckpointHistory> DbSetCheckpointHistory { get; set; }
    }
}