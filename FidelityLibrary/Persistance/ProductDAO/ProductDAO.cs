using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Products;
using FidelityLibrary.Persistance.Generics;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Persistance.ProductDAO
{
    public class ProductDAO : GenericDAO<Product, int>
    {
        public static void SaveProduct(Product Product, ApplicationDbContext oContext)
        {
            try
            {
                oContext.Entry(Product).State = EntityState.Added;
                oContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Transaction insert error: " + e);
            }
        }
    }
}
