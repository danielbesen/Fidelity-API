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
        public APIResult<Object> Add(LoyaltViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        var oLoyalt = new Loyalt()
                        {
                            Name = Model.Name,
                            Description = Model.Description,
                            Limit = Model.Limit,
                            EndDate = Model.EndDate,
                            EnterpriseId = Model.EnterpriseId,
                            FidelityTypeId = Model.FidelityTypeId,
                            PromotionTypeId = Model.PromotionTypeId,
                            ProductId = Model.ProductId,
                            ProductIdList = Model.ProductList,
                            Quantity = Model.Quantity,
                            StartDate = Model.StartDate
                        };

                        LoyaltyDAO.SaveLoyalt(oLoyalt, context);

                        if (Model.ProductList?.Count > 0)
                        {
                            foreach (var item in Model.ProductList)
                            {
                                var oFidelity = new FidelityLibrary.Entity.Fidelitys.Fidelity()
                                {
                                    ConsumedProductId = item,
                                    LoyaltId = oLoyalt.Id,
                                };
                                FidelityDAO.SaveFidelity(oFidelity, context);
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
                    var oLoyaltList = new List<LoyaltViewModel>();
                    foreach (var item in LoyaltyDAO.FindAll().ToList())
                    {
                        oLoyaltList.Add(new LoyaltViewModel()
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
                            StartDate = item.StartDate
                        });
                    }

                    return new APIResult<List<LoyaltViewModel>>()
                    {
                        Result = oLoyaltList,
                        Count = oLoyaltList.Count
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
                    var oLoyalt = LoyaltyDAO.FindByKey(Model.Id);

                    oLoyalt.Name = Model.Name;
                    oLoyalt.Description = Model.Description;
                    oLoyalt.Limit = Model.Limit;
                    oLoyalt.EndDate = Model.EndDate;
                    oLoyalt.FidelityTypeId = Model.FidelityTypeId;
                    oLoyalt.PromotionTypeId = Model.PromotionTypeId;
                    oLoyalt.ProductId = Model.ProductId;
                    oLoyalt.Quantity = Model.Quantity;
                    oLoyalt.StartDate = Model.StartDate;
                    oLoyalt.AlterDate = DateTime.Now;

                    LoyaltyDAO.Update(oLoyalt);

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
                        var Fidelities = FidelityDAO.FindAll().Where(x => x.LoyaltId == Id).ToList();
                        foreach (var fidel in Fidelities)
                        {
                            FidelityDAO.Delete(fidel);
                        }

                        var oLoyalt = LoyaltyDAO.FindByKey(Id);
                        LoyaltyDAO.Delete(oLoyalt);

                        return new APIResult<object>()
                        {
                            Message = "Fidelidade deletada com sucesso!"
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
                    Message = "Erro ao deletar fidelidade! " + e.Message + e.InnerException
                };
            }
        }
    }
}