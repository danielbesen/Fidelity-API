using Fidelity.Areas.Loyalts.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Loyalts;
using FidelityLibrary.Persistance.FidelityDAO;
using FidelityLibrary.Persistance.LoyaltyDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Loyalts.Controllers
{
    public class LoyaltController : ApiController
    {
        /// <summary>
        /// Requisição para inserir uma fidelidade ao sistema
        /// </summary>
        /// <returns>Client List Object></returns>
        [HttpPost]
        [Authorize]
        [Route("new/loyalt")]
        public APIResult<Object> NewLoyalt(LoyaltViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        var oFidelity = new FidelityLibrary.Entity.Fidelitys.Fidelity()
                        {
                            FidelityTypeId = Model.FidelityTypeId,
                            PromotionTypeId = Model.PromotionType,
                            ConsumedProductId = Model.ConsumedProductId, //Produto que precisará ser consumido
                            Quantity = Model.Quantity
                        };

                        FidelityDAO.SaveFidelity(oFidelity, context);

                        var oLoyalt = new Loyalt()
                        {
                            Name = Model.Name,
                            StartDate = Model.StartDate,
                            EndDate = Model.EndDate,
                            Description = Model.Description,
                            Limit = Model.Limit,
                            EnterpriseId = Model.EnterpriseId,
                            FidelityId = oFidelity.Id,
                            ProductId = Model.ProductId //Produto que o usuário irá ganhar, se houver
                        };

                        LoyaltyDAO.SaveLoyalt(oLoyalt, context);

                        dbContextTransaction.Commit();

                        return new APIResult<Object>()
                        {
                            Message = "Fidelidade cadastrada com sucesso!"
                        };
                    }
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao cadastrar fidelidade: " + e.Message
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
        public APIResult<List<LoyaltViewModel>> GetLoyalts()
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
                            Name = item.Name
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
                    Message = "Erro ao buscar todas fidelidades: " + e.Message,
                };
            }
        }
    }
}