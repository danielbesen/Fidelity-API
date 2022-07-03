using Fidelity.Models;
using FidelityLibrary.Persistance.LoyaltProgressDAO;
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
                    var Loyalts = LoyaltProgressDAO.FindAll().Where(x => x.EnterpriseId == company).GroupBy(x => x.LoyaltId).OrderByDescending(y => y.Count()).SelectMany(z => z).Select(a => a.LoyaltId).Distinct().ToList();

                }

                //return new APIResult<object>()
                //{
                //    Result = oUserList,
                //    Count = oUserList.Count
                //};

                return null;

            }
            catch (Exception e)
            {
                return new APIResult<object>()
                {
                    Success = false,
                    Message = "Erro ao buscar todos funcionários: " + e.Message + e.InnerException
                };
            }
        }
    }
}