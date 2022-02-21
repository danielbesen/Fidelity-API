using Fidelity.Areas.Products.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Products;
using FidelityLibrary.Persistance.LoyaltyDAO;
using FidelityLibrary.Persistance.ProductDAO;
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
        /// <param name="Model"></param>
        /// <returns>Client List Object></returns>
        [HttpPost]
        [Authorize]
        [Route("new/product")]
        public APIResult<Object> NewProduct(ProductViewModel Model)
        {
            try
            {
                var oProduct = new Product()
                {
                    EnterpriseId = Model.EnterpriseId,
                    Description = Model.Name,
                    Value = Model.Value,
                    CategoryId = Model.CategoryId,
                    Image = Model.Image,
                    Status = Model.Status
                };

                if (Model.LoyaltList?.Count > 0) //Se a lista de fidelidades vinculadas for maior que zero, salvar nova linha de fidelidade/fidelização
                {
                    //adicionar uma linha com a fidelidade selecionada, mas com o produtoID criado
                    foreach (var item in Model.LoyaltList)
                    {
                        var x = 2;
                    }
                }

                using (var context = new ApplicationDbContext())
                {
                    ProductDAO.Insert(oProduct);
                }

                return new APIResult<object>()
                {
                    Message = "Produto cadastrado com sucesso!"
                };

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

        /// <summary>
        /// Requisição para buscar todos os clientes no sistema.
        /// </summary>
        /// <returns>Client List Object></returns>
        [HttpGet]
        [Authorize]
        [Route("products")]
        public APIResult<List<ProductViewModel>> GetProducts(PaginationParams Params)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var oProductList = new List<ProductViewModel>();
                    foreach (var item in ProductDAO.FindAll().Skip(Params.Page - 1 * Params.PageSize).Take(Params.PageSize).ToList())
                    {
                        oProductList.Add(new ProductViewModel()
                        {
                            EnterpriseId = item.EnterpriseId,
                            Name = item.Description,
                            CategoryId = item.CategoryId,
                            Value = item.Value,
                            Image = item.Image,
                            Status = item.Status
                        });
                    }

                    return new APIResult<List<ProductViewModel>>()
                    {
                        Result = oProductList,
                        Count = oProductList.Count
                    };
                }
                else
                    return new APIResult<List<ProductViewModel>>()
                    {
                        Success = false,
                        Message = "Acesso negado!"
                    };

            }
            catch (Exception e)
            {
                return new APIResult<List<ProductViewModel>>()
                {
                    Success = false,
                    Message = "Erro ao buscar produtos! " + e.Message
                };
            }
        }
    }
}