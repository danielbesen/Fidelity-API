using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Users;
using FidelityLibrary.Models;
using FidelityLibrary.Persistance.Generics;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Persistance.UserDAO
{
    public class UserDAO : GenericDAO<User, int>
    {
        public static User GetUser(string email)
        {
            try
            {
                var LoginUser = new User();
                using (var context = new ApplicationDbContext())
                {
                    LoginUser = context.DbSetUser.AsNoTracking().FirstOrDefault(x => x.Email == email);
                }

                return LoginUser;
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao buscar usuário no banco: " + e.Message);
            }
        }

        public static void SaveUser(User User, ApplicationDbContext oContext)
        {
            try
            {
                oContext.Entry(User).State = EntityState.Added;
                oContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Transaction insert error: " + e);
            }
        }
    }
}
