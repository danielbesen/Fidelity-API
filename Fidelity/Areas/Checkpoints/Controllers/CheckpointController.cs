using Fidelity.Areas.Checkpoints.Models;
using Fidelity.Areas.Loyalts.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Checkpoints;
using FidelityLibrary.Entity.Loyalts;
using FidelityLibrary.Persistance.CheckpointDAO;
using FidelityLibrary.Persistance.ClientDAO;
using FidelityLibrary.Persistance.LoyaltProgressDAO;
using FidelityLibrary.Persistance.LoyaltyDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                    var oFidelity = LoyaltyDAO.FindByKey(Model.LoyaltId);
                    var oFidelityType = oFidelity.FidelityTypeId;
                    var oFidelityQtde = oFidelity.Quantity; //Quantidade à ser alcançada

                    var LastLoyaltProgress = LoyaltProgressDAO.FindAll().LastOrDefault(x => x.ClientId == Model.ClientId && x.LoyaltId == Model.LoyaltId);
                    double oPoints = 0;
                    bool oStatus = false;

                    if (oFidelityType == 2) //Pontuação
                    {
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
                            oStatus = true;
                            oPoints = 0;
                        }

                        var oProgress = new LoyaltProgress()
                        {
                            ClientId = Model.ClientId,
                            Points = oPoints,
                            Status = oStatus,
                            LoyaltId = Model.LoyaltId,
                        };

                        LoyaltProgressDAO.Insert(oProgress);

                        return new APIResult<LoyaltProgressViewModel>()
                        {
                            Message = "Checkpoint realizado com sucesso!",
                            Result = new LoyaltProgressViewModel()
                            {
                                ClientId = Model.ClientId,
                                LoyaltId = Model.LoyaltId,
                                Id = oProgress.Id,
                                Points = oPoints,
                                Status = oStatus
                            }
                        };
                    }
                    else //Valor
                    {
                        if (LastLoyaltProgress != null)
                        {
                            oPoints = LastLoyaltProgress.Points + Model.Value;
                        }
                        else
                        {
                            oPoints = Model.Value;
                        }

                        if (oPoints >= oFidelityQtde) //Ganhou
                        {
                            oStatus = true;
                            oPoints = 0;
                        }

                        var oProgress = new LoyaltProgress()
                        {
                            ClientId = Model.ClientId,
                            LoyaltId = Model.LoyaltId,
                            Points = oPoints,
                            Status = oStatus,
                        };

                        LoyaltProgressDAO.Insert(oProgress); //Insiro como true

                        //oProgress.Status = false;
                        //LoyaltProgressDAO.Insert(oProgress); // Insiro como false para resetar

                        return new APIResult<LoyaltProgressViewModel>()
                        {
                            Message = "Checkpoint realizado com sucesso!",
                            Result = new LoyaltProgressViewModel()
                            {
                                ClientId = Model.ClientId,
                                Id = oProgress.Id,
                                LoyaltId = oProgress.LoyaltId,
                                Points = oPoints,
                                Status = oStatus
                            }
                        };
                    }
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


        /// <summary>
        /// Requisição para buscar progresso de cliente no sistema.
        /// </summary>
        /// <returns>APIResult List Object></returns>
        [HttpGet]
        [Authorize]
        [Route("checkpoints")]
        public APIResult<List<LoyaltProgressViewModel>> Get()
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

                    var cpf = "";
                    if (parameters.ContainsKey("cpf"))
                    {
                        cpf = parameters["cpf"];
                    }

                    #endregion

                    if (string.IsNullOrEmpty(cpf))
                    {
                        return new APIResult<List<LoyaltProgressViewModel>>()
                        {
                            Success = false,
                            Message = "Nenhum cpf informado!"
                        };
                    }
                    var oClientId = ClientDAO.FindByCPF(cpf).Id;
                    var ClientProgressList = LoyaltProgressDAO.FindAll().Where(x => x.ClientId == oClientId).ToList();

                    // Pegar última linha de onde o id_fidelidade for o mesmo
                    var ListProgressListLast = new List<LoyaltProgress>();
                    var ListProgressListLastVM = new List<LoyaltProgressViewModel>();
                    var ListOfLoyaltIds = new List<int>();

                    foreach (var item in ClientProgressList)
                    {
                        if (!ListOfLoyaltIds.Contains(item.LoyaltId))
                        {
                            ListOfLoyaltIds.Add(item.LoyaltId);
                        }
                    }

                    foreach (var item in ListOfLoyaltIds)
                    {
                        ListProgressListLast.Add(ClientProgressList.LastOrDefault(x => x.LoyaltId == item));
                    }

                    if (ListProgressListLast == null)
                    {
                        return new APIResult<List<LoyaltProgressViewModel>>()
                        {
                            Success = false,
                            Message = "Nenhum progresso encontrado",
                        };
                    }

                    foreach (var item in ListProgressListLast)
                    {
                        ListProgressListLastVM.Add(new LoyaltProgressViewModel()
                        {
                            Id = item.Id,
                            ClientId = item.ClientId,
                            LoyaltId = item.LoyaltId,
                            Points = item.Points,
                            Status = item.Status
                        });
                    }

                    return new APIResult<List<LoyaltProgressViewModel>>()
                    {
                        Message = "Busca de progressos realizada com sucesso!",
                        Result = ListProgressListLastVM,
                        Count = ListProgressListLastVM.Count
                    };

                }
            }
            catch (Exception e)
            {
                return new APIResult<List<LoyaltProgressViewModel>>()
                {
                    Success = false,
                    Message = "Erro ao buscar progresso de cliente! " + e.Message + e.InnerException
                };
            }
        }
    }
}