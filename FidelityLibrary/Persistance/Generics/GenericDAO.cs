using FidelityLibrary.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Persistance.Generics
{
    /// <summary>
    /// Generic BD Operations
    /// </summary>
    /// <typeparam name="T">Class</typeparam>
    /// <typeparam name="K">ID Key</typeparam>
    public class GenericDAO<T, K> where T : class
    {
        public static void Insert(T Entity)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    context.Entry(Entity).State = EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                throw new DbEntityValidationException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + e.InnerException.InnerException.Message);
            }
        }

        public static void Update(T Entity)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    context.Entry(Entity).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                throw new DbEntityValidationException(e.Message + e.InnerException);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + e.InnerException);
            }
        }

        public static void Delete(T Entity)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    context.Entry(Entity).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                throw new DbEntityValidationException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static T FindByKey(K Id)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    return context.Set<T>().Find(Id);
                }
            }
            catch (DbEntityValidationException e)
            {
                throw new DbEntityValidationException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao carregar objeto do banco - " + e.Message);
            }
        }

        public static List<T> FindAll()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    return context.Set<T>().AsNoTracking().ToList();
                }
            }
            catch (DbEntityValidationException e)
            {
                throw new DbEntityValidationException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}