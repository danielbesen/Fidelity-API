using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Fidelitys;
using FidelityLibrary.Persistance.Generics;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Persistance.FidelityDAO
{
    public class FidelityDAO : GenericDAO<Fidelity, int>
    {
        public static void SaveFidelity(Fidelity Fidelity, ApplicationDbContext oContext)
        {
            try
            {
                oContext.Entry(Fidelity).State = EntityState.Added;
                oContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Transaction insert error: " + e);
            }
        }
    }
}
