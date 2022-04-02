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
        public APIResult<List<ClientViewModel>> Get()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var oClientList = new List<ClientViewModel>();

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

                    if (!string.IsNullOrEmpty(cpf))
                    {
                        var oClient = ClientDAO.FindAll().FirstOrDefault(x => x.Cpf == cpf);

                        foreach (var item in ClientDAO.FindAll().Where(x => x.Cpf == cpf).ToList())
                        {
                            oClientList.Add(new ClientViewModel()
                            {
                                Id = item.Id,
                                Cpf = item.Cpf,
                                Name = item.Name
                            });
                        }
                    }
                    else
                    {
                        foreach (var item in ClientDAO.FindAll().ToList())
                        {
                            oClientList.Add(new ClientViewModel()
                            {
                                Id = item.Id,
                                Cpf = item.Cpf,
                                Name = item.Name
                            });
                        }
                    }

                    return new APIResult<List<ClientViewModel>>()
                    {
                        Result = oClientList,
                        Count = oClientList.Count
                    };
                }
                else
                    return new APIResult<List<ClientViewModel>>()
                    {
                        Success = false,
                        Message = "Acesso negado!"
                    };
            }
            catch (Exception e)
            {
                return new APIResult<List<ClientViewModel>>()
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
                                    Cpf = Model.Client.Cpf
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

    }
}