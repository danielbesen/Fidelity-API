using Fidelity.Areas.Products.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Products.Controllers
{
    public class ProductController : ApiController
    {
        /// <summary>
        /// Requisição para buscar todos os clientes no sistema.
        /// </summary>
        /// <returns>Client List Object></returns>
        [HttpGet]
        [Authorize]
        [Route("new/product")]
        public APIResult<Object> NewProduct(ProductViewModel Model)
        {
            try
            {
                var oProduct = new Product()
                {
                    Description = Model.ProductName,
                    Value = Model.Value,
                    Category = Model.Category,
                    Image = Model.Image,
                    Status = Model.status,
                    
                };


                using (var context = new ApplicationDbContext())
                {

                }

            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao criar produto! " + e.Message
                };
            }
        }
    }
}