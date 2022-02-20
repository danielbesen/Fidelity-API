using Fidelity.Areas.Categories.Models;
using Fidelity.Models;
using FidelityLibrary.Persistance.CategoryDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Categories.Controllers
{
    public class CategoryController : ApiController
    {
        /// <summary>
        /// Requisição para buscar todas as categorias no sistema.
        /// </summary>
        /// <returns>Category List Object></returns>
        [HttpGet]
        [Authorize]
        [Route("categories")]
        public APIResult<List<CategoryViewModel>> GetCategories()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var oCategoryList = new List<CategoryViewModel>();
                    foreach (var item in CategoryDAO.FindAll().ToList())
                    {
                        oCategoryList.Add(new CategoryViewModel()
                        {
                            Id = item.Id,
                            Name = item.Name
                        });
                    }

                    return new APIResult<List<CategoryViewModel>>()
                    {
                        Result = oCategoryList,
                        Count = oCategoryList.Count
                    };
                }
                else
                    return new APIResult<List<CategoryViewModel>>()
                    {
                        Success = false,
                        Message = "Acesso negado!"
                    };
            }
            catch (Exception e)
            {
                return new APIResult<List<CategoryViewModel>>()
                {
                    Success = false,
                    Message = "Erro ao buscar todas categorias: " + e.Message,
                };
            }
        }
    }
}