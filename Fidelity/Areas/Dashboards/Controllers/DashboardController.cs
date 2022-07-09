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

                if (company != 0)
                {
                    var Loyalts = LoyaltProgressDAO.FindAll().Where(x => x.EnterpriseId == company && x.InsertDate > DateTime.Today.AddDays(-30)).GroupBy(x => x.LoyaltId).OrderByDescending(y => y.Count()).SelectMany(z => z).Select(a => a.LoyaltId).ToList();
                    var TotalClients = LoyaltProgressDAO.FindAll().Where(x => x.EnterpriseId == company).Select(x => x.ClientId).Distinct().Count();
                    //var BestProduct = LoyaltProgressDAO.FindAll().Where(x => x.EnterpriseId == company).Select()
                    if (Loyalts.Count > 0)
                    {
                        #region Most Used Loyalts

                        var MostUsedLoyaltList = new List<MostUsedLoyaltsViewModel>();
                        var dict = Loyalts.GroupBy(x => x).ToDictionary(x => x.Key, q => q.Count());
                        foreach (var item in Loyalts.Distinct())
                        {
                            var Loyalt = LoyaltyDAO.FindByKey(item);

                            MostUsedLoyaltList.Add(new MostUsedLoyaltsViewModel()
                            {
                                Name = Loyalt.Name,
                                Number = dict[item]
                            });

                        }

                        #endregion



                        var Dashboard = new EnterpriseDashboardViewModel()
                        {
                            TotalClients = TotalClients,
                            MostUsedLoyalts = MostUsedLoyaltList,
                        };

                        return new APIResult<object>()
                        {
                            Message = "Sucesso ao buscar as fidelidades mais utilizadas nos últimos 30 dias!",
                            Result = Dashboard,
                            Count = MostUsedLoyaltList.Count()
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
                } else
                {
                    return new APIResult<object>()
                    {
                        Success = false,
                        Message = "Nenhuma empresa informada!"
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