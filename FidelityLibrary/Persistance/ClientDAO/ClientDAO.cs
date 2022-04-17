using FidelityLibrary.DataContext;
using FidelityLibrary.Entity;
using FidelityLibrary.Persistance.Generics;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Persistance.ClientDAO
{
    public class ClientDAO : GenericDAO<Client, int>
    {
        public static Client FindByUserId(int UserId)
        {
            try
            {
                var oClient = new Client();
                using (var context = new ApplicationDbContext())
                {
                    oClient = context.DbSetClient.FirstOrDefault(x => x.UserId == UserId);
                };

                return oClient;
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao buscar cliente pelo Id de usuário: " + e.Message);
            }
        }

        public static Client FindByCPF(string Cpf)
        {
            try
            {
                var oClient = new Client();
                using (var context = new ApplicationDbContext())
                {
                    oClient = context.DbSetClient.FirstOrDefault(x => x.Cpf == Cpf);
                };

                return oClient;
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao buscar cliente pelo CPF: " + e.Message);
            }
        }

        public static void SaveClient(Client Client, ApplicationDbContext oContext)
        {
            try
            {
                oContext.Entry(Client).State = EntityState.Added;
                oContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Transaction insert error: " + e);
            }
        }

    }
}
