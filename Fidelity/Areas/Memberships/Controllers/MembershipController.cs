using Fidelity.Areas.Memberships.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity.Memberships;
using FidelityLibrary.Persistance.MembershipDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Memberships.Controllers
{
    public class MembershipController : ApiController
    {
        /// <summary>
        /// Requisição para buscar todos os planos no sistema.
        /// </summary>
        /// <returns>APIResult List Object></returns>
        [HttpGet]
        [Route("memberships")]
        public APIResult<List<MembershipViewModel>> Get()
        {
            try
            {
                    var oMemberList = new List<MembershipViewModel>();
                    foreach (var item in MembershipDAO.FindAll().ToList())
                    {
                        oMemberList.Add(new MembershipViewModel()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Description = item.Description,
                            Value = item.Value
                        });
                    }

                    return new APIResult<List<MembershipViewModel>>()
                    {
                        Result = oMemberList,
                        Count = oMemberList.Count
                    };
            }
            catch (Exception e)
            {
                return new APIResult<List<MembershipViewModel>>()
                {
                    Success = false,
                    Message = "Erro ao buscar todos os planos: " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para adicionar um plano no sistema.
        /// </summary>
        /// <returns>APIResult List Object></returns>
        [HttpPost]
        [Authorize]
        [Route("memberships")]
        public APIResult<Object> Add(MembershipViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var oMembership = new Membership()
                    {
                        Name = Model.Name,
                        Value = Model.Value,
                        Description = Model.Description
                    };

                    MembershipDAO.Insert(oMembership);

                    return new APIResult<object>()
                    {
                        Message = "Plano cadastrado com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao criar plano! " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para atualizar um plano no sistema.
        /// </summary>
        /// <returns>APIResult List Object></returns>
        [HttpPut]
        [Authorize]
        [Route("memberships")]
        public APIResult<Object> Update(MembershipViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var oMembership = MembershipDAO.FindByKey(Model.Id);

                    oMembership.Name = Model.Name;
                    oMembership.Value = Model.Value;
                    oMembership.Description = Model.Description;
                    oMembership.AlterDate = DateTime.Now;

                    MembershipDAO.Update(oMembership);

                    return new APIResult<object>()
                    {
                        Message = "Plano atualizado com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao atualizar plano! " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para deletar um plano no sistema.
        /// </summary>
        /// <returns>APIResult List Object></returns>
        [HttpDelete]
        [Authorize]
        [Route("memberships")]
        public APIResult<Object> Delete()
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

                    var Id = 0;

                    if (parameters.ContainsKey("id"))
                    {
                        Id = Int32.Parse(parameters["id"]);
                    }

                    #endregion

                    if (Id != 0)
                    {
                        var oMembership = MembershipDAO.FindByKey(Id);

                        MembershipDAO.Delete(oMembership);

                        return new APIResult<object>()
                        {
                            Message = "Plano deletado com sucesso!"
                        };
                    } else
                    {
                        return new APIResult<object>()
                        {
                            Success = false,
                            Message = "Nenhum ID informado!"
                        };
                    }
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao deletar plano! " + e.Message + e.InnerException
                };
            }
        }
    }
}