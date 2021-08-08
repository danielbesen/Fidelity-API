using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Users;
using FidelityLibrary.Models;
using FidelityLibrary.Persistance.Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Persistance.UserDAO
{
    public class UserDAO : GenericDAO<User, int>
    {
        public static User GetUser(User LoginUser)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    LoginUser = context.DbSetUser.AsNoTracking().FirstOrDefault(x => x.Email == LoginUser.Email);
                }

                return LoginUser;
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao buscar usuário no banco: " + e.Message);
            }
        }
    }
}
