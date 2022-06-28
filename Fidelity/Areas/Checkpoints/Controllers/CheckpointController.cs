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
using System.Security.Claims;
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
        public APIResult<List<LoyaltProgressViewModel>> Add(CheckpointListViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var oListProgressVM = new List<LoyaltProgressViewModel>();

                    foreach (var checkpoint in Model.Checkpoints)
                    {
                        var oFidelity = LoyaltyDAO.FindByKey(checkpoint.LoyaltId);
                        var oFidelityType = oFidelity.FidelityTypeId;
                        var oFidelityQtde = oFidelity.Quantity; //Quantidade à ser alcançada

                        var LastLoyaltProgress = LoyaltProgressDAO.FindAll().LastOrDefault(x => x.ClientId == checkpoint.ClientId && x.LoyaltId == checkpoint.LoyaltId && x.EnterpriseId == checkpoint.EnterpriseId);
                        double oPoints = 0;
                        bool oStatus = false;

                        if (LastLoyaltProgress != null)
                        {
                            oPoints = LastLoyaltProgress.Points + checkpoint.Value;
                        }
                        else
                        {
                            oPoints = checkpoint.Value;
                        }

                        if (oPoints >= oFidelityQtde) //Ganhou
                        {
                            oStatus = true;
                            oPoints = 0;
                        }

                        var oProgress = new LoyaltProgress()
                        {
                            ClientId = checkpoint.ClientId,
                            Points = oPoints,
                            Status = oStatus,
                            LoyaltId = checkpoint.LoyaltId,
                            EnterpriseId = checkpoint.EnterpriseId,
                        };

                        var oLoyaltP = new LoyaltProgress();
                        if (LastLoyaltProgress != null)
                        {
                            oLoyaltP = LoyaltProgressDAO.FindByKey(LastLoyaltProgress.Id);
                            oLoyaltP.Status = oStatus;
                            oLoyaltP.Points = oPoints;
                            LoyaltProgressDAO.Update(oLoyaltP);
                        } else
                        {
                            LoyaltProgressDAO.Insert(oProgress);
                        }

                        oListProgressVM.Add(new LoyaltProgressViewModel()
                        {
                            ClientId = checkpoint.ClientId,
                            LoyaltId = checkpoint.LoyaltId,
                            Id = oProgress.Id != null ? oProgress.Id : oLoyaltP.Id,
                            Points = oPoints,
                            Status = oStatus
                        });
                    }

                    return new APIResult<List<LoyaltProgressViewModel>>()
                    {
                        Message = "Checkpoint(s) realizado(s) com sucesso!",
                        Count = oListProgressVM.Count,
                        Result = oListProgressVM
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<List<LoyaltProgressViewModel>>()
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
                    var ListProgressListLastVM = new List<LoyaltProgressViewModel>();


                    if (!ClientProgressList.Any())
                    {
                        return new APIResult<List<LoyaltProgressViewModel>>()
                        {
                            Success = false,
                            Message = "Nenhum progresso encontrado!",
                        };
                    }

                    foreach (var item in ClientProgressList)
                    {
                        var oLoyaltDB = LoyaltyDAO.FindByKey(item.LoyaltId);
                        var oClientDB = ClientDAO.FindByKey(item.ClientId);

                        ListProgressListLastVM.Add(new LoyaltProgressViewModel()
                        {
                            Id = item.Id,
                            ClientId = item.ClientId,
                            LoyaltId = item.LoyaltId,
                            Points = item.Points,
                            Status = item.Status,
                            Client = new Clients.Models.ClientViewModel()
                            {
                                Name = oClientDB.Name,
                                Cpf = oClientDB.Cpf,
                                Id = oClientDB.Id,
                                UserId = oClientDB.UserId
                            },
                            Loyalt = new LoyaltViewModel()
                            {
                                Id = oLoyaltDB.Id,
                                CouponValue = oLoyaltDB.CouponValue,
                                Name = oLoyaltDB.Name,
                                Limit = oLoyaltDB.Limit,
                                ProductId = oLoyaltDB.ProductId,
                                Description = oLoyaltDB.Description,
                                EnterpriseId = oLoyaltDB.EnterpriseId,
                                FidelityTypeId = oLoyaltDB.FidelityTypeId,
                                PromotionTypeId = oLoyaltDB.PromotionTypeId,
                                Quantity = oLoyaltDB.Quantity,
                                StartDate = oLoyaltDB.StartDate,
                                EndDate = oLoyaltDB.EndDate
                            }
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