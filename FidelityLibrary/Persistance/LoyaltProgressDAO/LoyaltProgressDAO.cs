using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Checkpoints;
using FidelityLibrary.Entity.Loyalts;
using FidelityLibrary.Persistance.Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Persistance.LoyaltProgressDAO
{
    public class LoyaltProgressDAO : GenericDAO<LoyaltProgress, int?>
    {
        public static LoyaltProgress FindByClientId(int ClientId)
        {
            try
            {
                var LoyaltProgress = new LoyaltProgress();
                using (var context = new ApplicationDbContext())
                {
                    LoyaltProgress = context.DbSetLoyaltProgress.FirstOrDefault(x => x.ClientId == ClientId);
                };

                return LoyaltProgress;
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao buscar progresso de fidelidade pelo Id de client: " + e.Message);
            }
        }
    }
}
