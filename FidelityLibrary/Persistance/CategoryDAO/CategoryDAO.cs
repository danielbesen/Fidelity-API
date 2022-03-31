using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Categories;
using FidelityLibrary.Persistance.Generics;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Persistance.CategoryDAO
{
    public class CategoryDAO : GenericDAO<Category, int>
    {
        public static void SaveCategory(Category Category, ApplicationDbContext oContext)
        {
            try
            {
                oContext.Entry(Category).State = EntityState.Added;
                oContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Transaction insert error: " + e);
            }
        }

        public static void PutCategory(Category Category, ApplicationDbContext oContext)
        {
            try
            {
                oContext.Entry(Category).State = EntityState.Modified;
                oContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Transaction insert error: " + e);
            }
        }

        public static void DeleteCategory(Category Category, ApplicationDbContext oContext)
        {
            try
            {
                oContext.Entry(Category).State = EntityState.Deleted;
                oContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Transaction insert error: " + e);
            }
        }
    }
}
