using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity;
using FidelityLibrary.Persistance.ClientDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Security.Claims;
using Fidelity.Areas.Clients.Models;
using FidelityLibrary.Persistance.UserDAO;
using FidelityLibrary.Entity.Users;
using FidelityLibrary.Models;
using FidelityLibrary.Persistance.Generics;
using Fidelity.Areas.Users.Models;
using Fidelity.Areas.Enterprises.Models;
using System.Net.Http;

namespace Fidelity.Areas.Clients.Controllers
{
    public class ClientController : ApiController
    {
        /// <summary>
        /// Requisição para buscar todos os clientes no sistema.
        /// </summary>
        /// <returns>Client List Object></returns>
        [HttpGet]
        [Authorize]
        [Route("clients")]
        public APIResult<List<UserViewModel>> Get()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
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

                    var oUserList = new List<UserViewModel>();

                    if (!string.IsNullOrEmpty(cpf))
                    {
                        foreach (var item in ClientDAO.FindAll().Where(x => x.Cpf == cpf).ToList())
                        {
                            var oClient = new ClientViewModel()
                            {
                                Id = item.Id,
                                Cpf = item.Cpf,
                                Name = item.Name,
                                UserId = item.UserId
                            };

                            var oUser = UserDAO.FindByKey(item.UserId);

                            oUserList.Add(new UserViewModel
                            {
                                Image = oUser.Image,
                                Active = oUser.Active,
                                Email = oUser.Email,
                                Type = oUser.Type,
                                Client = oClient,
                            });
                        }
                    }
                    else
                    {
                        foreach (var item in ClientDAO.FindAll().ToList())
                        {
                            var oClient = new ClientViewModel()
                            {
                                Id = item.Id,
                                Cpf = item.Cpf,
                                Name = item.Name,
                                UserId = item.UserId
                            };

                            var oUser = UserDAO.FindByKey(item.UserId);

                            oUserList.Add(new UserViewModel
                            {
                                Image = oUser.Image,
                                Active = oUser.Active,
                                Email = oUser.Email,
                                Type = oUser.Type,
                                Client = oClient,
                            });
                        }
                    }

                    return new APIResult<List<UserViewModel>>()
                    {
                        Result = oUserList,
                        Count = oUserList.Count
                    };
                }
                else
                    return new APIResult<List<UserViewModel>>()
                    {
                        Success = false,
                        Message = "Acesso negado!"
                    };
            }
            catch (Exception e)
            {
                return new APIResult<List<UserViewModel>>()
                {
                    Success = false,
                    Message = "Erro ao buscar todos clientes: " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para cadastrar cliente no sistema.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns>API Result Object</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("clients")]
        public APIResult<Object> SignUp(UserViewModel Model)
        {
            try
            {
                if (UserDAO.FindAll().ToList().Any(x => x.Email == Model.Email))
                {
                    return new APIResult<Object>()
                    {
                        Success = false,
                        Message = "E-mail já cadastrado!",
                    };
                }
                else
                {
                    #region Saving User and Client

                    try
                    {
                        using (var context = new ApplicationDbContext())
                        {
                            using (var dbContextTransaction = context.Database.BeginTransaction())
                            {

                                var user = new User()
                                {
                                    Email = Model.Email.ToLower(),
                                    Type = "C",
                                    Active = "1",
                                    Password = Encrypt.EncryptPass(Model.Password)
                                };

                                UserDAO.SaveUser(user, context);

                                var client = new Client()
                                {
                                    UserId = user.Id,
                                    Name = Model.Client.Name,
                                    Cpf = Model.Client.Cpf.Replace(".","").Replace("-", "")
                                };

                                ClientDAO.SaveClient(client, context);

                                dbContextTransaction.Commit();

                                return new APIResult<Object>()
                                {
                                    Message = "Cliente cadastrado com sucesso!"
                                };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        return new APIResult<Object>()
                        {
                            Success = false,
                            Message = "Erro na transação: " + e.Message + e.InnerException
                        };
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao validar Login: " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para atualizar cliente no sistema.
        /// </summary>
        /// <returns>Product List Object></returns>
        [HttpPut]
        [Authorize]
        [Route("clients")]
        public APIResult<Object> Update(UserViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var oUser = UserDAO.FindByKey(Model.Client.UserId);

                    if (Model.Password != null)
                    {
                        oUser.Password = Encrypt.EncryptPass(Model.Password);
                    }

                    oUser.Email = Model.Email;
                    oUser.Image = Model.Image;
                    oUser.Active = Model.Active;
                    oUser.AlterDate = DateTime.Now;

                    var oClient = ClientDAO.FindByKey(Model.Client.Id);
                    oClient.Name = Model.Client.Name;
                    oClient.Cpf = Model.Client.Cpf.Replace(".", "").Replace("-", "");
                    oClient.AlterDate = DateTime.Now;

                    ClientDAO.Update(oClient);

                    return new APIResult<object>()
                    {
                        Message = "Cliente atualizado com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao atualizar cliente: " + e.Message + e.InnerException
                };
            }
        }

    }
}