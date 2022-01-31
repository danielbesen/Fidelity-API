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
                            FidelityTypeId = Model.FidelityId,
                            PromotionTypeId = Model.PromotionType,
                            ProductId = Model.ProductId, //Produto que precisará ser consumido
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
    }
}