using Fidelity.Areas.Categories.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Persistance.CategoryDAO;
using FidelityLibrary.Persistance.ProductDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Categories.Controllers
{
    public class CategoryController : ApiController
    {
        /// <summary>
        /// Requisição para buscar todas as categorias no sistema.
        /// </summary>
        /// <returns>APIResult List Object></returns>
        [HttpGet]
        [Authorize]
        [Route("categories")]
        public APIResult<List<CategoryViewModel>> Get()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var company = 0;
                    var identity = User.Identity as ClaimsIdentity;
                    if (identity != null)
                    {
                        company = Convert.ToInt32(identity.FindFirst("company").Value);
                    }

                    var CategoryList = new List<CategoryViewModel>();
                    foreach (var item in CategoryDAO.FindAll().Where(x => x.EnterpriseId == company && x.Status).ToList())
                    {
                        CategoryList.Add(new CategoryViewModel()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            EnterpriseId = item.EnterpriseId,
                            Status = item.Status
                        });
                    }

                    return new APIResult<List<CategoryViewModel>>()
                    {
                        Result = CategoryList,
                        Count = CategoryList.Count
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
        /// Requisição para adicionar uma categoria no sistema.
        /// </summary>
        /// <returns>APIResult List Object></returns>
        [HttpPost]
        [Authorize]
        [Route("categories")]
        public APIResult<Object> Add(CategoryViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var Category = new FidelityLibrary.Entity.Categories.Category()
                    {
                        Name = Model.Name,
                        EnterpriseId = Model.EnterpriseId,
                        Status = Model.Status 
                    };

                    CategoryDAO.Insert(Category);

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
        /// Requisição para atualizar uma categoria no sistema.
        /// </summary>
        /// <returns>APIResult List Object></returns>
        [HttpPut]
        [Authorize]
        [Route("categories")]
        public APIResult<Object> Update(CategoryViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var Category = CategoryDAO.FindByKey(Model.Id);

                    Category.Name = Model.Name ?? "";
                    Category.AlterDate = DateTime.Now;
                    Category.Status = Model.Status;

                    CategoryDAO.Update(Category);

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
        /// Requisição para deletar uma categoria no sistema.
        /// </summary>
        /// <returns>APIResult List Object></returns>
        [HttpDelete]
        [Authorize]
        [Route("categories")]
        public APIResult<Object> Delete()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    #region GET PARAMS

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    foreach (var parameter in Request.GetQueryNameValuePairs())
                    {
                        parameters.Add(parameter.Key, parameter.Value);
                    }

                    var Id = 0;

                    if (parameters.ContainsKey("id"))
                    {
                        Id = Int32.Parse(parameters["id"]);
                    }

                    #endregion

                    if (Id != 0)
                    {
                        var Category = CategoryDAO.FindByKey(Id);
                        Category.Status = false;
                        CategoryDAO.Update(Category);

                        var ListOfProducts = ProductDAO.FindAll().Where(x => x.CategoryId == Id && x.Status).ToList();
                        foreach (var product in ListOfProducts)
                        {
                            product.CategoryId = null;
                            ProductDAO.Update(product);
                        }

                        return new APIResult<object>()
                        {
                            Message = "Categoria deletada com sucesso!"
                        };
                    } else
                    {
                        return new APIResult<object>()
                        {
                            Success = false,
                            Message = "Nenhum ID informado!"
                        };
                    }
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