using Fidelity.Areas.Fidelities.Models;
using Fidelity.Areas.Loyalts.Models;
using Fidelity.Areas.Products.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Products;
using FidelityLibrary.Persistance.FidelityDAO;
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
                using (var context = new ApplicationDbContext())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
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

                        ProductDAO.SaveProduct(oProduct, context);

                        if (Model.FidelityList?.Count > 0) //Se a lista de fidelidades vinculadas for maior que zero, salvar nova linha de fidelização
                        {
                            //adicionar uma linha com a fidelização selecionada, mas com o produtoID criado
                            foreach (var item in Model.FidelityList)
                            {
                                var DbFidelity = FidelityDAO.FindByKey(item);
                                if (DbFidelity != null)
                                {
                                    DbFidelity.ConsumedProductId = oProduct.Id;
                                }

                                FidelityDAO.SaveFidelity(DbFidelity, context);
                            }
                        }
                        dbContextTransaction.Commit();
                    }

                    return new APIResult<object>()
                    {
                        Message = "Produto cadastrado com sucesso!"
                    };

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
                    var Products = new List<Product>();
                    if (Params == null)
                    {
                        Products = ProductDAO.FindAll().ToList();
                    }
                    else
                    {
                        Products = ProductDAO.FindAll().Skip((Params.Page - 1) * Params.PageSize).Take(Params.PageSize).ToList();
                    }

                    var oProductList = new List<ProductViewModel>();
                    foreach (var item in Products)
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
                        Count = oProductList.Count,
                        Message = "Sucesso ao buscar produtos!"
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