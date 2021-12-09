using FidelityLibrary.DataContext;
using FidelityLibrary.Entity;
using FidelityLibrary.Persistance.Generics;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Persistance.EnterpriseDAO
{
    public class EnterpriseDAO : GenericDAO<Enterprise, int>
    {
        public static Enterprise FindByUserId(int UserId)
        {
            try
            {
                var oEnterprise = new Enterprise();
                using (var context = new ApplicationDbContext())
                {
                    oEnterprise = context.DbSetEnterprise.FirstOrDefault(x => x.UserId == UserId);
                };

                return oEnterprise;
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao buscar empresa pelo Id de usuário: " + e.Message);
            }
        }

        public static void SaveEnterprise(Enterprise Enterprise, ApplicationDbContext oContext)
        {
            try
            {
                oContext.Entry(Enterprise).State = EntityState.Added;
                oContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Transaction insert error: " + e);
            }
        }
    }
}
