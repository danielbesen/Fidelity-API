using Fidelity.Areas.Enterprises.Models;
using Fidelity.Areas.Loyalts.Models;
using Fidelity.Areas.Products.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Loyalts;
using FidelityLibrary.Entity.Products;
using FidelityLibrary.Persistance.FidelityDAO;
using FidelityLibrary.Persistance.LoyaltyDAO;
using FidelityLibrary.Persistance.ProductDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Loyalts.Controllers
{
    public class LoyaltController : ApiController
    {
        /// <summary>
        /// Requisição para inserir uma fidelidade ao sistema.
        /// </summary>
        /// <returns>Client List Object></returns>
        [HttpPost]
        [Authorize]
        [Route("loyalts")]
        public APIResult<object> Add(LoyaltViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        var Loyalt = new Loyalt()
                        {
                            Name = Model.Name,
                            Description = Model.Description,
                            Limit = Model.Limit,
                            EndDate = Model.EndDate,
                            EnterpriseId = Model.EnterpriseId,
                            FidelityTypeId = Model.FidelityTypeId,
                            PromotionTypeId = Model.PromotionTypeId,
                            ProductId = Model.ProductId,
                            CouponValue = Model.CouponValue,
                            Quantity = Model.Quantity,
                            StartDate = Model.StartDate,
                            Status = Model.Status
                        };

                        LoyaltyDAO.SaveLoyalt(Loyalt, context);

                        if (Model.ProductList?.Count > 0)
                        {
                            foreach (var item in Model.ProductList)
                            {
                                var Fidelity = new FidelityLibrary.Entity.Fidelitys.Fidelity()
                                {
                                    ConsumedProductId = item,
                                    LoyaltId = Loyalt.Id,
                                    Status = true
                                };
                                FidelityDAO.SaveFidelity(Fidelity, context);
                                System.Threading.Thread.Sleep(100);
                            }
                        }

                        dbContextTransaction.Commit();
                    }

                    return new APIResult<Object>()
                    {
                        Message = "Fidelidade cadastrada com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao cadastrar fidelidade: " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para buscar todas as fidelidades no sistema.
        /// </summary>
        /// <returns>Loyalt List Object></returns>
        [HttpGet]
        [Authorize]
        [Route("loyalts")]
        public APIResult<List<LoyaltViewModel>> Get()
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

                    var company = 0;
                    var identity = User.Identity as ClaimsIdentity;
                    if (identity.FindFirst("company") != null)
                    {
                        company = Convert.ToInt32(identity.FindFirst("company").Value);
                    }
                    else
                    {
                        if (parameters.ContainsKey("company"))
                        {
                            company = Int32.Parse(parameters["company"]);
                        } else
                        {
                            return new APIResult<List<LoyaltViewModel>>()
                            {
                                Success = false,
                                Message = "Nenhuma empresa informada!"
                            };
                        }
                    }

                    var LoyaltList = new List<Loyalt>();

                    if (!string.IsNullOrEmpty(name))
                    {
                        LoyaltList = LoyaltyDAO.FindAll().Where(x => x.Name.ToLower().Contains(name) && x.EnterpriseId == company && x.Status).ToList();
                    }
                    else
                    {
                        if (page == 0)
                        {
                            LoyaltList = LoyaltyDAO.FindAll().Where(x => x.EnterpriseId == company && x.Status).ToList();
                        }
                        else
                        {
                            LoyaltList = LoyaltyDAO.FindAll().Where(x => x.EnterpriseId == company && x.Status).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                        }
                    }


                    var LoyaltListVM = new List<LoyaltViewModel>();
                    foreach (var item in LoyaltList)
                    {

                        #region Product

                        var oProductList = new List<ProductViewModel>();

                        var oFidelities = FidelityDAO.FindAll().Where(x => x.LoyaltId == item.Id && x.Status).ToList();
                        foreach (var id in oFidelities)
                        {
                            var oProduct = ProductDAO.FindByKey(id.ConsumedProductId);

                            oProductList.Add(new ProductViewModel()
                            {
                                Id = oProduct.Id,
                                Name = oProduct.Description,
                                Image = oProduct.Image,
                                Status = oProduct.Status,
                                Value = id.ConsumedProductId,
                                CategoryId = id.ConsumedProductId
                            });
                        }

                        #endregion

                        LoyaltListVM.Add(new LoyaltViewModel()
                        {
                            Id = item.Id,
                            Description = item.Description,
                            EnterpriseId = item.EnterpriseId,
                            Limit = item.Limit,
                            Name = item.Name,
                            ProductId = item.ProductId,
                            FidelityTypeId = item.FidelityTypeId,
                            Quantity = item.Quantity,
                            PromotionTypeId = item.PromotionTypeId,
                            EndDate = item.EndDate,
                            StartDate = item.StartDate,
                            CouponValue = item.CouponValue,
                            Status = item.Status,
                            Products = oProductList
                        });
                    }

                    return new APIResult<List<LoyaltViewModel>>()
                    {
                        Result = LoyaltListVM,
                        Count = LoyaltListVM.Count
                    };
                }
                else
                    return new APIResult<List<LoyaltViewModel>>()
                    {
                        Success = false,
                        Message = "Acesso negado!"
                    };
            }
            catch (Exception e)
            {
                return new APIResult<List<LoyaltViewModel>>()
                {
                    Success = false,
                    Message = "Erro ao buscar todas fidelidades: " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para atualizar uma fidelidade.
        /// </summary>
        /// <returns>APIResult Object></returns>
        [HttpPut]
        [Authorize]
        [Route("loyalts")]
        public APIResult<Object> Update(LoyaltViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var Loyalt = LoyaltyDAO.FindByKey(Model.Id);

                    Loyalt.Name = Model.Name;
                    Loyalt.Description = Model.Description;
                    Loyalt.Limit = Model.Limit;
                    Loyalt.EndDate = Model.EndDate;
                    Loyalt.FidelityTypeId = Model.FidelityTypeId;
                    Loyalt.PromotionTypeId = Model.PromotionTypeId;
                    Loyalt.ProductId = Model.ProductId;
                    Loyalt.Quantity = Model.Quantity;
                    Loyalt.CouponValue = Model.CouponValue;
                    Loyalt.Status = Model.Status;
                    Loyalt.StartDate = Model.StartDate;
                    Loyalt.AlterDate = DateTime.Now;

                    var ExistingProductsIds = FidelityDAO.FindAll().Where(x => x.LoyaltId == Loyalt.Id).ToDictionary(x => x.ConsumedProductId, q => q.Status);

                    foreach (var item in Model.ProductList)
                    {
                        if (!ExistingProductsIds.Keys.Contains(item))
                        {
                            var Fidelity = new FidelityLibrary.Entity.Fidelitys.Fidelity()
                            {
                                ConsumedProductId = item,
                                LoyaltId = Loyalt.Id,
                                Status = true
                            };

                            FidelityDAO.Insert(Fidelity);
                            System.Threading.Thread.Sleep(300);
                        } else
                        {
                            if (!ExistingProductsIds[item])
                            {
                                var Fidel = FidelityDAO.FindAll().FirstOrDefault(x => x.LoyaltId == Model.Id && x.ConsumedProductId == item);
                                Fidel.Status = true;
                                FidelityDAO.Update(Fidel);
                                System.Threading.Thread.Sleep(100);
                            }
                        }
                    }

                    foreach (var item in ExistingProductsIds.Keys)
                    {
                        if (!Model.ProductList.Contains(item) && ExistingProductsIds[item])
                        {
                            var Fidel = FidelityDAO.FindAll().FirstOrDefault(x => x.LoyaltId == Model.Id && x.ConsumedProductId == item);
                            Fidel.Status = false;
                            FidelityDAO.Update(Fidel);
                            System.Threading.Thread.Sleep(100);
                        }
                    }

                    LoyaltyDAO.Update(Loyalt);

                    return new APIResult<Object>()
                    {
                        Message = "Fidelidade atualizada com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao atualizar fidelidade: " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para deletar uma fidelidade.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns>APIResult Object></returns>
        [HttpDelete]
        [Authorize]
        [Route("loyalts")]
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
                        var Fidelities = FidelityDAO.FindAll().Where(x => x.LoyaltId == Id && x.Status).ToList();
                        foreach (var fidel in Fidelities)
                        {
                            fidel.Status = false;
                            FidelityDAO.Update(fidel);
                        }

                        var Loyalt = LoyaltyDAO.FindByKey(Id);
                        Loyalt.Status = false;
                        LoyaltyDAO.Update(Loyalt);

                        return new APIResult<object>()
                        {
                            Message = "Fidelidade deletada com sucesso!"
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
                    Message = "Erro ao deletar fidelidade! " + e.Message + e.InnerException
                };
            }
        }
    }
}