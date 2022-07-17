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
                    var ListProgressVM = new List<LoyaltProgressViewModel>();

                    foreach (var checkpoint in Model.Checkpoints)
                    {
                        var Fidelity = LoyaltyDAO.FindByKey(checkpoint.LoyaltId);
                        var FidelityQtde = Fidelity.Quantity; //Quantidade à ser alcançada

                        var LastLoyaltProgress = LoyaltProgressDAO.FindAll().LastOrDefault(x => x.ClientId == checkpoint.ClientId && x.LoyaltId == checkpoint.LoyaltId && x.EnterpriseId == checkpoint.EnterpriseId);
                        double Points = 0;
                        bool CheckPointStatus = false;

                        if (LastLoyaltProgress != null)
                        {
                            Points = LastLoyaltProgress.Points + checkpoint.Value;
                        }
                        else
                        {
                            Points = checkpoint.Value;
                        }

                        if (Points >= FidelityQtde) //Ganhou
                        {
                            CheckPointStatus = true;
                            Points = 0;

                            var Historic = new CheckpointHistory()
                            {
                                ClientId = checkpoint.ClientId,
                                EnterpriseId = checkpoint.EnterpriseId,
                                LoyaltId = checkpoint.LoyaltId,
                            };

                            CheckpointHistoryDAO.Insert(Historic);
                        }

                        var oProgress = new LoyaltProgress()
                        {
                            ClientId = checkpoint.ClientId,
                            Points = Points,
                            Status = CheckPointStatus,
                            LoyaltId = checkpoint.LoyaltId,
                            EnterpriseId = checkpoint.EnterpriseId,
                        };

                        var oLoyaltP = new LoyaltProgress();
                        if (LastLoyaltProgress != null)
                        {
                            oLoyaltP = LoyaltProgressDAO.FindByKey(LastLoyaltProgress.Id);
                            oLoyaltP.Status = CheckPointStatus;
                            oLoyaltP.Points = Points;
                            oLoyaltP.InsertDate = DateTime.Now;
                            LoyaltProgressDAO.Update(oLoyaltP);
                        } else
                        {
                            LoyaltProgressDAO.Insert(oProgress);
                        }

                        ListProgressVM.Add(new LoyaltProgressViewModel()
                        {
                            ClientId = checkpoint.ClientId,
                            LoyaltId = checkpoint.LoyaltId,
                            Id = oProgress.Id != null ? oProgress.Id : oLoyaltP.Id,
                            Points = Points,
                            Status = CheckPointStatus
                        });
                    }

                    return new APIResult<List<LoyaltProgressViewModel>>()
                    {
                        Message = "Checkpoint(s) realizado(s) com sucesso!",
                        Count = ListProgressVM.Count,
                        Result = ListProgressVM
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

                    var company = 0;
                    var identity = User.Identity as ClaimsIdentity;
                    if (identity.FindFirst("company") != null)
                    {
                        company = Convert.ToInt32(identity.FindFirst("company").Value);
                    }

                    var ClientId = ClientDAO.FindByCPF(cpf)?.Id;
                    var ClientProgressList = company == 0 ? LoyaltProgressDAO.FindAll().Where(x => x.ClientId == ClientId).ToList() : LoyaltProgressDAO.FindAll().Where(x => x.ClientId == ClientId && x.EnterpriseId == company).ToList();
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
                        var LoyaltDB = LoyaltyDAO.FindByKey(item.LoyaltId);
                        var ClientDB = ClientDAO.FindByKey(item.ClientId);

                        ListProgressListLastVM.Add(new LoyaltProgressViewModel()
                        {
                            Id = item.Id,
                            ClientId = item.ClientId,
                            LoyaltId = item.LoyaltId,
                            Points = item.Points,
                            Status = item.Status,
                            Client = new Clients.Models.ClientViewModel()
                            {
                                Name = ClientDB.Name,
                                Cpf = ClientDB.Cpf,
                                Id = ClientDB.Id,
                                UserId = ClientDB.UserId
                            },
                            Loyalt = new LoyaltViewModel()
                            {
                                Id = LoyaltDB.Id,
                                CouponValue = LoyaltDB.CouponValue,
                                Name = LoyaltDB.Name,
                                Limit = LoyaltDB.Limit,
                                ProductId = LoyaltDB.ProductId,
                                Description = LoyaltDB.Description,
                                EnterpriseId = LoyaltDB.EnterpriseId,
                                FidelityTypeId = LoyaltDB.FidelityTypeId,
                                PromotionTypeId = LoyaltDB.PromotionTypeId,
                                Quantity = LoyaltDB.Quantity,
                                StartDate = LoyaltDB.StartDate,
                                EndDate = LoyaltDB.EndDate
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