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
        public APIResult<List<Client>> Get()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return new APIResult<List<Client>>()
                    {
                        Result = ClientDAO.FindAll().ToList(),
                        Count = ClientDAO.FindAll().ToList().Count
                    };
                }
                else
                    return new APIResult<List<Client>>()
                    {
                        Success = false,
                        Message = "Acesso negado!"
                    };
            }
            catch (Exception e)
            {
                return new APIResult<List<Client>>()
                {
                    Success = false,
                    Message = "Erro ao buscar todos clientes: " + e.Message,
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
        [Route("signup/client")]
        public APIResult<string> Signup(UserViewModel Model)
        {
            try
            {
                if (UserDAO.FindAll().ToList().Any(x => x.Email == Model.Email))
                {
                    return new APIResult<string>()
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
                                    Name = Model.Name,
                                    Email = Model.Email,
                                    Type = Model.Type,
                                    Active = "1",
                                    Password = Encrypt.EncryptPass(Model.Password)
                                };

                                UserDAO.SaveUser(user, context);

                                var client = new Client()
                                {
                                    UserId = UserDAO.FindAll().FirstOrDefault(x => x.Email == Model.Email)?.Id,
                                    Name = Model.Client.Name,
                                    Cpf = Model.Client.Cpf
                                };

                                ClientDAO.SaveClient(client, context);

                                dbContextTransaction.Commit();

                                return new APIResult<string>()
                                {
                                    Message = "Cliente cadastrado com sucesso!"
                                };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Transaction insert error: " + e);
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                return new APIResult<string>()
                {
                    Success = false,
                    Message = "Erro ao validar Login: " + e.Message,
                };
            }
        }
    }
}