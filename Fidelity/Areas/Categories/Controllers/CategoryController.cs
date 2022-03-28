using Fidelity.Areas.Categories.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
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
        public APIResult<List<CategoryViewModel>> Get()
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
                    Message = "Erro ao buscar todas categorias: " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para cadastar uma nova categoria.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns>Category List Object></returns>
        [HttpPost]
        [Authorize]
        [Route("categories")]
        public APIResult<Object> Add(CategoryViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var oCategory = new FidelityLibrary.Entity.Categories.Category()
                    {
                        Name = Model.Name
                    };

                    CategoryDAO.Insert(oCategory);

                    return new APIResult<object>()
                    {
                        Message = "Categoria cadastrada com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao criar categoria! " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para alterar uma categoria.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns>Category List Object></returns>
        [HttpPut]
        [Authorize]
        [Route("categories")]
        public APIResult<Object> Update(CategoryViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var oCategory = CategoryDAO.FindByKey(Model.Id);

                    oCategory.Name = Model.Name;
                    oCategory.AlterDate = DateTime.Now;

                    CategoryDAO.Update(oCategory);

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
                    Message = "Erro ao atualizar categoria! " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para deletar uma categoria.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns>Category List Object></returns>
        [HttpDelete]
        [Authorize]
        [Route("categories")]
        public APIResult<Object> Delete(CategoryViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var oCategory = CategoryDAO.FindByKey(Model.Id);

                    CategoryDAO.Delete(oCategory);

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
                    Message = "Erro ao deletar categoria! " + e.Message + e.InnerException
                };
            }
        }
    }
}