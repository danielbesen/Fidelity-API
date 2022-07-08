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
                        var Product = new Product()
                        {
                            EnterpriseId = Model.EnterpriseId,
                            Description = Model.Name,
                            Value = Model.Value,
                            CategoryId = Model.CategoryId,
                            Image = Model.Image,
                            Status = Model.Status
                        };

                        ProductDAO.SaveProduct(Product, context);

                        if (Model.LoyaltList?.Count > 0) //Se a lista de fidelidades vinculadas for maior que zero, salvar nova linha de fidelização
                        {
                            //adicionar uma linha com a fidelização selecionada, mas com o produtoID criado
                            foreach (var item in Model.LoyaltList)
                            {
                                var Fidelity = new FidelityLibrary.Entity.Fidelitys.Fidelity()
                                {
                                    ConsumedProductId = Product.Id,
                                    LoyaltId = item,
                                    Status = true
                                };
                                FidelityDAO.SaveFidelity(Fidelity, context);
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

                    if (parameters.ContainsKey("name"))
                    {
                        name = parameters["name"].ToLower();
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
                        Products = ProductDAO.FindAll().Where(x => x.Description.ToLower().Contains(name) && x.EnterpriseId == company && x.Status).ToList();
                    }
                    else
                    {
                        if (page == 0)
                        {
                            Products = ProductDAO.FindAll().Where(x => x.EnterpriseId == company && x.Status).ToList();
                        }
                        else
                        {
                            Products = ProductDAO.FindAll().Where(x => x.EnterpriseId == company && x.Status).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                        }
                    }

                    var ProductList = new List<ProductViewModel>();
                    foreach (var item in Products)
                    {
                        var Category = item.CategoryId != null ? CategoryDAO.FindByKey(item.CategoryId) : null;
                        var LoyaltListIds = FidelityDAO.FindAll().Where(x => x.ConsumedProductId == item.Id && x.Status).ToList();

                        var LoyaltsVM = new List<LoyaltViewModel>();

                        foreach (var loyalt in LoyaltListIds)
                        {
                            var Loyal = LoyaltyDAO.FindByKey(loyalt.LoyaltId);

                            LoyaltsVM.Add(new LoyaltViewModel()
                            {
                                Id = Loyal.Id,
                                Description = Loyal.Description,
                                Name = Loyal.Name,
                                EnterpriseId = Loyal.EnterpriseId,
                                ProductId = Loyal.ProductId,
                                Limit = Loyal.Limit,
                                Quantity = Loyal.Quantity,
                                StartDate = Loyal.StartDate,
                                EndDate = Loyal.EndDate,
                                FidelityTypeId = Loyal.FidelityTypeId,
                                ConsumedProductId = Loyal.FidelityTypeId,
                                PromotionTypeId = Loyal.PromotionTypeId
                            });
                        }

                        var CategoryVM = new CategoryViewModel();

                        if (Category != null) 
                        {
                            CategoryVM.Id = Category.Id;
                            CategoryVM.Name = Category.Name;
                            CategoryVM.DataAlteracao = Category.AlterDate;
                            CategoryVM.DataInclusao = Category.InsertDate;
                            CategoryVM.Status = Category.Status;
                        }

                        ProductList.Add(new ProductViewModel()
                        {
                            Id = item.Id,
                            EnterpriseId = item.EnterpriseId,
                            Name = item.Description,
                            CategoryId = item.CategoryId,
                            Value = item.Value,
                            Image = item.Image,
                            Status = item.Status,
                            Category = Category != null ? CategoryVM : null,
                            Loyalts = LoyaltsVM
                        });
                    }

                    return new APIResult<List<ProductViewModel>>()
                    {
                        Result = ProductList,
                        Count = ProductList.Count,
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
                    var Product = ProductDAO.FindByKey(Model.Id);

                    Product.Description = Model.Name;
                    Product.Status = Model.Status;
                    Product.CategoryId = Model.CategoryId;
                    Product.Image = Model.Image;
                    Product.AlterDate = DateTime.Now;
                    Product.Value = Model.Value;
                    Product.Status = Model.Status;

                    var ExistingLoyalts = FidelityDAO.FindAll().Where(x => x.ConsumedProductId == Product.Id).ToDictionary(x => x.LoyaltId, q => q.Status);

                    foreach (var item in Model.LoyaltList)
                    {
                        if (!ExistingLoyalts.Keys.Contains(item))
                        {
                            var Fidelity = new FidelityLibrary.Entity.Fidelitys.Fidelity()
                            {
                                ConsumedProductId = Product.Id,
                                LoyaltId = item,
                                Status = true
                            };

                            FidelityDAO.Insert(Fidelity);
                            System.Threading.Thread.Sleep(300);
                        }
                        else
                        {
                            if (!ExistingLoyalts[item])
                            {
                                var Fidel = FidelityDAO.FindAll().FirstOrDefault(x => x.LoyaltId == item && x.ConsumedProductId == Model.Id);
                                Fidel.Status = true;
                                FidelityDAO.Update(Fidel);
                                System.Threading.Thread.Sleep(100);
                            }
                        }
                    }

                    foreach (var item in ExistingLoyalts.Keys)
                    {
                        if (!Model.LoyaltList.Contains(item) && ExistingLoyalts[item]) {
                            var Fidel = FidelityDAO.FindAll().FirstOrDefault(x => x.LoyaltId == item && x.ConsumedProductId == Model.Id);
                            Fidel.Status = false;
                            FidelityDAO.Update(Fidel);
                            System.Threading.Thread.Sleep(100);
                        }
                    }

                    ProductDAO.Update(Product);

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

                        var Fidelities = FidelityDAO.FindAll().Where(x => x.ConsumedProductId == Id && x.Status).ToList();
                        foreach (var fidel in Fidelities)
                        {
                            fidel.Status = false;
                            FidelityDAO.Update(fidel);
                        }

                        var Product = ProductDAO.FindByKey(Id);
                        Product.Status = false;

                        ProductDAO.Update(Product);

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