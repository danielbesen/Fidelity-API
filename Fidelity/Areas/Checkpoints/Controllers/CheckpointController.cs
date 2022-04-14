using Fidelity.Areas.Checkpoints.Models;
using Fidelity.Areas.Loyalts.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Checkpoints;
using FidelityLibrary.Entity.Loyalts;
using FidelityLibrary.Persistance.CheckpointDAO;
using FidelityLibrary.Persistance.LoyaltProgressDAO;
using FidelityLibrary.Persistance.LoyaltyDAO;
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
        public APIResult<LoyaltProgressViewModel> Add(CheckpointViewModel Model)
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

                    var oFidelity = LoyaltyDAO.FindByKey(Model.LoyaltId);
                    var oFidelityType = oFidelity.FidelityTypeId;
                    var oFidelityQtde = oFidelity.Quantity; //Quantidade à ser alcançada

                    if (oFidelityType == 2) //Pontuação
                    {
                        var LastLoyaltProgress = LoyaltProgressDAO.FindAll().LastOrDefault(x => x.ClientId == Model.ClientId);
                        int oPoints = 0;
                        int oStatus = 0;

                        if (LastLoyaltProgress != null)
                        {
                            oPoints = LastLoyaltProgress.Points + 1;
                        }
                        else
                        {
                            oPoints = 1;
                        }

                        if (oPoints == oFidelityQtde) //Ganhou
                        {
                            oStatus = 1;
                            oPoints = 0;
                        }

                        var oProgress = new LoyaltProgress()
                        {
                            ClientId = Model.ClientId,
                            CheckpointId = oCheckpoint.Id,
                            Points = oPoints,
                            Status = oStatus,
                        };

                        LoyaltProgressDAO.Insert(oProgress);

                        return new APIResult<LoyaltProgressViewModel>()
                        {
                            Message = "Checkpoint realizado com sucesso!",
                            Result = new LoyaltProgressViewModel()
                            {
                                ClientId = Model.ClientId,
                                Id = oProgress.Id,
                                CheckpointId = oProgress.CheckpointId,
                                Points = oPoints,
                                Status = oStatus
                            }
                    };
                } else if (oFidelityType == 3) //Valor
                {

                }

                return new APIResult<LoyaltProgressViewModel>()
                {
                    Message = "Checkpoint realizado com sucesso!"
                };
            }
            }
            catch (Exception e)
            {
                return new APIResult<LoyaltProgressViewModel>()
                {
                    Success = false,
                    Message = "Erro ao realizar checkpoint! " + e.Message + e.InnerException
    };
}
        }
    }
}