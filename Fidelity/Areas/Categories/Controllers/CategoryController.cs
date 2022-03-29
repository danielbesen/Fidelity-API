using Fidelity.Areas.Categories.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Categories;
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
        [Route("category")]
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

        [HttpPost]
        [Authorize]
        [Route("category")]
        public APIResult<Object> NewCategory(CategoryViewModel model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {

                    var categoria = new Category()
                    {
                        Name = model.Name,
                        InsertDate = model.DataInclusao,
                    };

                    CategoryDAO.SaveCategory(categoria,context);

                    return new APIResult<object>()
                    {
                        Message = "Categoria cadastrado com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao buscar todas categorias: " + e.Message,
                };
            }
        }

        [HttpPut]
        [Authorize]
        [Route("category")]
        public APIResult<Object> PutCategory(CategoryViewModel model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var categoria = new Category()
                    {
                        Id = model.Id,
                        AlterDate = model.DataAlteracao,
                        Name = model.Name,
                    };

                    CategoryDAO.PutCategory(categoria,context);

                    return new APIResult<object>()
                    {
                        Message = "Categoria atualizada com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao buscar todas categorias: " + e.Message,
                };
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("category")]
        public APIResult<Object> DeleteCategory(CategoryViewModel model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var categoria = new Category()
                    {
                        Id = model.Id
                    };

                    CategoryDAO.DeleteCategory(categoria, context);

                    return new APIResult<object>()
                    {
                        Message = "Categoria deletada com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao buscar todas categorias: " + e.Message,
                };
            }
        }
    }
}