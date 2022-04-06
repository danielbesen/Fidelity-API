using Fidelity.Areas.Checkpoints.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Checkpoints;
using FidelityLibrary.Entity.Loyalts;
using FidelityLibrary.Persistance.CheckpointDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Checkpoints.Controllers
{
    public class CheckpointController : ApiController
    {
        /// <summary>
        /// Requisição para realizar um checkpoint no sistema.
        /// </summary>
        /// <returns>APIResult List Object></returns>
        [HttpPost]
        [Authorize]
        [Route("checkpoints")]
        public APIResult<Object> Add(CheckpointViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var oCheckpoint = new Checkpoint()
                    {
                        ClientId = Model.ClientId,
                        LoyaltId = Model.LoyaltId,
                    };

                    CheckpointDAO.Insert(oCheckpoint);

                    var oProgress = new LoyaltProgress()
                    {
                        ClientId = Model.ClientId,
                        id_checkpoint = oCheckpoint.Id,
                        //Points = 

                    };

                    return new APIResult<object>()
                    {
                        Message = "Checkpoint realizado com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao realizar checkpoint! " + e.Message + e.InnerException
                };
            }
        }
    }
}