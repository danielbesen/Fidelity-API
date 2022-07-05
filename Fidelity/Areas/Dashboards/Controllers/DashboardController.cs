using Fidelity.Areas.Dashboards.Models;
using Fidelity.Areas.Loyalts.Models;
using Fidelity.Models;
using FidelityLibrary.Entity.Loyalts;
using FidelityLibrary.Persistance.LoyaltProgressDAO;
using FidelityLibrary.Persistance.LoyaltyDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Dashboards.Controllers
{
    public class DashboardController : ApiController
    {
        /// <summary>
        /// Requisição para buscar dados de inteligência no sistema.
        /// </summary>
        /// <returns>></returns>
        [HttpGet]
        [Authorize]
        [Route("dashboard")]
        public APIResult<object> Get()
        {
            try
            {
                var company = 0;
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    company = Convert.ToInt32(identity.FindFirst("company").Value);
                }

                var Loyalts = new List<int>();

                if (company != 0)
                {
                    Loyalts = LoyaltProgressDAO.FindAll().Where(x => x.EnterpriseId == company).GroupBy(x => x.LoyaltId).OrderByDescending(y => y.Count()).SelectMany(z => z).Select(a => a.LoyaltId).ToList();
                }

                if (Loyalts.Count > 0)
                {
                    var oDashboardList = new List<EnterpriseDashboardViewModel>();
                    var dict = Loyalts.GroupBy(x => x).ToDictionary(x => x.Key, q => q.Count());
                    foreach (var item in Loyalts.Distinct())
                    {
                        var oLoyalt = LoyaltyDAO.FindByKey(item);

                        oDashboardList.Add(new EnterpriseDashboardViewModel()
                        {
                            Name = oLoyalt.Name,
                            Number = dict[item]
                        });
                    }

                    return new APIResult<object>()
                    {
                        Result = oDashboardList,
                        Count = oDashboardList.Count()
                    };
                }
                else
                {
                    return new APIResult<object>()
                    {
                        Success = false,
                        Message = "Nenhuma fidelidade para essa empresa encontrada!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<object>()
                {
                    Success = false,
                    Message = "Erro ao buscar dados: " + e.Message + e.InnerException
                };
            }
        }
    }
}