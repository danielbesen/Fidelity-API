using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Loyalts;
using FidelityLibrary.Persistance.Generics;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Persistance.LoyaltyDAO
{
    public class LoyaltyDAO : GenericDAO<Loyalt, int>
    {
        public static void SaveLoyalt(Loyalt Loyalt, ApplicationDbContext oContext)
        {
            try
            {
                oContext.Entry(Loyalt).State = EntityState.Added;
                oContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Transaction insert error: " + e);
            }
        }
    }
}
