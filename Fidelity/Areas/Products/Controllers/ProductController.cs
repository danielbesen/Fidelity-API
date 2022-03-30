using Fidelity.Areas.Fidelities.Models;
using Fidelity.Areas.Loyalts.Models;
using Fidelity.Areas.Products.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Products;
using FidelityLibrary.Persistance.FidelityDAO;
using FidelityLibrary.Persistance.LoyaltyDAO;
using FidelityLibrary.Persistance.ProductDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Products.Controllers
{
    public class ProductController : ApiController
    {
        /// <summary>
        /// Requisição para cadastrar um novo produto
        /// </summary>
        /// <param name="Model"></param>
        /// <returns>APIResult Object></returns>
        [HttpPost]
        [Authorize]
        [Route("products")]
        public APIResult<Object> Add(ProductViewModel Model)
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

                        if (Model.LoyaltList?.Count > 0) //Se a lista de fidelidades vinculadas for maior que zero, salvar nova linha de fidelização
                        {
                            //adicionar uma linha com a fidelização selecionada, mas com o produtoID criado
                            foreach (var item in Model.LoyaltList)
                            {
                                var oFidelity = new FidelityLibrary.Entity.Fidelitys.Fidelity()
                                {
                                    ConsumedProductId = oProduct.Id,
                                    LoyaltId = item
                                };
                                FidelityDAO.SaveFidelity(oFidelity, context);
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
                    Message = "Erro ao criar produto! " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para buscar todos os produtos no sistema.
        /// </summary>
        /// <returns>Product List Object></returns>
        [HttpGet]
        [Authorize]
        [Route("products")]
        public APIResult<List<ProductViewModel>> Get()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    #region GET PARAMS

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    foreach (var parameter in Request.GetQueryNameValuePairs())
                    {
                        parameters.Add(parameter.Key, parameter.Value);
                    }

                    var name = "";
                    var page = 0;
                    var pageSize = 0;

                    if (parameters.ContainsKey("name"))
                    {
                        name = parameters["name"];
                    }

                    if (parameters.ContainsKey("page"))
                    {
                        page = Int32.Parse(parameters["page"]);
                    }

                    if (parameters.ContainsKey("pagesize"))
                    {
                        pageSize = Int32.Parse(parameters["pagesize"]);
                    }

                    #endregion

                    #region GET

                    var Products = new List<Product>();

                    if (!string.IsNullOrEmpty(name))
                    {
                        Products = ProductDAO.FindAll().Where(x => x.Description.ToLower().Contains(name)).ToList();
                    }
                    else
                    {

                        if (page == 0)
                        {
                            Products = ProductDAO.FindAll().ToList();
                        }
                        else
                        {
                            Products = ProductDAO.FindAll().Skip((page - 1) * pageSize).Take(pageSize).ToList();
                        }
                    }

                    var oProductList = new List<ProductViewModel>();
                    foreach (var item in Products)
                    {
                        oProductList.Add(new ProductViewModel()
                        {
                            Id = item.Id,
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

                    #endregion
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
                    Message = "Erro ao buscar produtos! " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para alterar um produto.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns>Product List Object></returns>
        [HttpPut]
        [Authorize]
        [Route("products")]
        public APIResult<Object> Update(ProductViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var oProduct = ProductDAO.FindByKey(Model.Id);

                    oProduct.Description = Model.Name;
                    oProduct.Status = Model.Status;
                    oProduct.CategoryId = Model.CategoryId;
                    oProduct.Image = Model.Image;
                    oProduct.AlterDate = DateTime.Now;
                    oProduct.Value = Model.Value;

                    ProductDAO.Update(oProduct);

                    return new APIResult<object>()
                    {
                        Message = "Produto atualizado com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao atualizar produto! " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para deletar um produto.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns>Product List Object></returns>
        [HttpDelete]
        [Authorize]
        [Route("products")]
        public APIResult<Object> Delete(ProductViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var Loyalts = LoyaltyDAO.FindAll().Where(x => x.ProductId == Model.Id).ToList();
                    if (Loyalts.Any())
                    {
                        return new APIResult<object>()
                        {
                            Success = false,
                            Message = "Produto vinculado como prêmio de fidelidades. Remova-as primeiro."
                        };
                    }

                    var Fidelities = FidelityDAO.FindAll().Where(x => x.ConsumedProductId == Model.Id).ToList();
                    foreach (var fidel in Fidelities)
                    {
                        FidelityDAO.Delete(fidel);
                    }

                    var oProduct = ProductDAO.FindByKey(Model.Id);

                    ProductDAO.Delete(oProduct);

                    return new APIResult<object>()
                    {
                        Message = "Produto deletado com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao deletar produto! " + e.Message + e.InnerException
                };
            }
        }
    }
}