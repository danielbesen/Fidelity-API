using Fidelity.Areas.Categories.Models;
using Fidelity.Areas.Fidelities.Models;
using Fidelity.Areas.Loyalts.Models;
using Fidelity.Areas.Products.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Products;
using FidelityLibrary.Persistance.CategoryDAO;
using FidelityLibrary.Persistance.FidelityDAO;
using FidelityLibrary.Persistance.LoyaltyDAO;
using FidelityLibrary.Persistance.ProductDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
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

                    var company = 0;
                    var identity = User.Identity as ClaimsIdentity;
                    if (identity != null)
                    {
                        company = Convert.ToInt32(identity.FindFirst("company").Value);
                    }

                    #region GET PARAMS

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    foreach (var parameter in Request.GetQueryNameValuePairs())
                    {
                        parameters.Add(parameter.Key, parameter.Value);
                    }

                    var name = "";
                    var page = 0;
                    var pageSize = 0;

                    //var company = 0;

                    //if (parameters.ContainsKey("company"))
                    //{
                    //    company = Int32.Parse(parameters["company"]);
                    //}
                    //else
                    //{
                    //    return new APIResult<List<ProductViewModel>>()
                    //    {
                    //        Success = false,
                    //        Message = "Nenhuma empresa informada!"
                    //    };
                    //}

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

                    var Products = new List<Product>();

                    if (!string.IsNullOrEmpty(name))
                    {
                        Products = ProductDAO.FindAll().Where(x => x.Description.ToLower().Contains(name) && x.EnterpriseId == company).ToList();
                    }
                    else
                    {
                        if (page == 0)
                        {
                            Products = ProductDAO.FindAll().Where(x => x.EnterpriseId == company).ToList();
                        }
                        else
                        {
                            Products = ProductDAO.FindAll().Where(x => x.EnterpriseId == company).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                        }
                    }

                    var oProductList = new List<ProductViewModel>();
                    foreach (var item in Products)
                    {
                        var oCategory = CategoryDAO.FindByKey(item.CategoryId);
                        var oLoyaltListIds = FidelityDAO.FindAll().Where(x => x.ConsumedProductId == item.Id).ToList();

                        var LoyaltsVM = new List<LoyaltViewModel>();

                        foreach (var loyalt in oLoyaltListIds)
                        {
                            var oLoyalt = LoyaltyDAO.FindByKey(loyalt.LoyaltId);

                            LoyaltsVM.Add(new LoyaltViewModel()
                            {
                                Id = oLoyalt.Id,
                                Description = oLoyalt.Description,
                                Name = oLoyalt.Name,
                                EnterpriseId = oLoyalt.EnterpriseId,
                                ProductId = oLoyalt.ProductId,
                                Limit = oLoyalt.Limit,
                                Quantity = oLoyalt.Quantity,
                                StartDate = oLoyalt.StartDate,
                                EndDate = oLoyalt.EndDate,
                                FidelityTypeId = oLoyalt.FidelityTypeId,
                                ConsumedProductId = oLoyalt.FidelityTypeId,
                                PromotionTypeId = oLoyalt.PromotionTypeId
                            });
                        }

                        var CategoryVM = new CategoryViewModel();

                        if (oCategory != null) 
                        {
                            CategoryVM.Id = oCategory.Id;
                            CategoryVM.Name = oCategory.Name;
                            CategoryVM.DataAlteracao = oCategory.AlterDate;
                            CategoryVM.DataInclusao = oCategory.InsertDate;
                        }

                        oProductList.Add(new ProductViewModel()
                        {
                            Id = item.Id,
                            EnterpriseId = item.EnterpriseId,
                            Name = item.Description,
                            CategoryId = item.CategoryId,
                            Value = item.Value,
                            Image = item.Image,
                            Status = item.Status,
                            Category = oCategory != null ? CategoryVM : null,
                            Loyalts = LoyaltsVM
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
                    oProduct.Status = Model.Status;

                    var oFidelities = FidelityDAO.FindAll().Where(x => x.ConsumedProductId == oProduct.Id).ToList();
                    foreach (var fidelity in oFidelities)
                    {
                        FidelityDAO.Delete(fidelity);
                    }

                    if (Model.LoyaltList?.Count > 0)
                    {
                        foreach (var item in Model.LoyaltList)
                        {
                            var oFidelity = new FidelityLibrary.Entity.Fidelitys.Fidelity()
                            {
                                ConsumedProductId = oProduct.Id,
                                LoyaltId = item
                            };
                            FidelityDAO.Insert(oFidelity);
                            System.Threading.Thread.Sleep(600);
                        }
                    }

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
                        var Loyalts = LoyaltyDAO.FindAll().Where(x => x.ProductId == Id).ToList();
                        if (Loyalts.Any())
                        {
                            return new APIResult<object>()
                            {
                                Success = false,
                                Message = "Produto vinculado como prêmio de fidelidades. Remova-as primeiro."
                            };
                        }

                        var Fidelities = FidelityDAO.FindAll().Where(x => x.ConsumedProductId == Id).ToList();
                        foreach (var fidel in Fidelities)
                        {
                            FidelityDAO.Delete(fidel);
                        }

                        var oProduct = ProductDAO.FindByKey(Id);

                        ProductDAO.Delete(oProduct);

                        return new APIResult<object>()
                        {
                            Message = "Produto deletado com sucesso!"
                        };
                    }
                    else
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
                    Message = "Erro ao deletar produto! " + e.Message + e.InnerException
                };
            }
        }
    }
}