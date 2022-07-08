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

                    var UserList = new List<UserViewModel>();

                    if (!string.IsNullOrEmpty(cpf))
                    {
                        foreach (var item in ClientDAO.FindAll().Where(x => x.Cpf == cpf).ToList())
                        {
                            var Client = new ClientViewModel()
                            {
                                Id = item.Id,
                                Cpf = item.Cpf,
                                Name = item.Name,
                                UserId = item.UserId
                            };

                            var User = UserDAO.FindByKey(item.UserId);

                            UserList.Add(new UserViewModel
                            {
                                Image = User.Image,
                                Status = User.Status,
                                Email = User.Email,
                                Type = User.Type,
                                Client = Client,
                            });
                        }
                    }
                    else
                    {
                        foreach (var item in ClientDAO.FindAll().Join(UserDAO.FindAll(), c => c.UserId, u => u.Id, (c, u) => new { C = c, U = u }).Where(CU => CU.C.UserId == CU.U.Id && CU.U.Status).ToList())
                        {
                            var Client = new ClientViewModel()
                            {
                                Id = item.C.Id,
                                Cpf = item.C.Cpf,
                                Name = item.C.Name,
                                UserId = item.C.UserId
                            };

                            var User = UserDAO.FindByKey(item.C.UserId);

                            UserList.Add(new UserViewModel
                            {
                                Image = User.Image,
                                Status = User.Status,
                                Email = User.Email,
                                Type = User.Type,
                                Client = Client,
                            });
                        }
                    }

                    return new APIResult<List<UserViewModel>>()
                    {
                        Result = UserList,
                        Count = UserList.Count
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
                                    Status = true,
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
                    #region GET PARAMS

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    foreach (var parameter in Request.GetQueryNameValuePairs())
                    {
                        parameters.Add(parameter.Key, parameter.Value);
                    }

                    var id = 0;
                    if (parameters.ContainsKey("id"))
                    {
                        id = Convert.ToInt32(parameters["id"]);
                    }

                    #endregion

                    var User = UserDAO.FindByKey(Model.Client.UserId);

                    if (Model.Password != null)
                    {
                        User.Password = Encrypt.EncryptPass(Model.Password);
                    }

                    User.Email = Model.Email;
                    User.Image = Model.Image;
                    User.Status = Model.Status;
                    User.AlterDate = DateTime.Now;

                    var Client = ClientDAO.FindByKey(Model.Client.Id);
                    Client.Name = Model.Client.Name;
                    Client.Cpf = Model.Client.Cpf.Replace(".", "").Replace("-", "");
                    Client.AlterDate = DateTime.Now;

                    ClientDAO.Update(Client);
                    UserDAO.Update(User);

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