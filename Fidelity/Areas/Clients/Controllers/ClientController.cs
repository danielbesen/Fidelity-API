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
        public APIResult<string> Signup(ClientViewModel Model)
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

                    #region Legacy Code

                    //fazer uma transaction aqui pra caso 1 insert der falha, voltar tudo
                    //UserDAO.Insert(new User()
                    //{
                    //    Email = Model.Email,
                    //////    Type = Model.Type,
                    //    Password = Encrypt.EncryptPass(Model.Password)
                    //});

                    //ClientDAO.Insert(new Client()
                    //{
                    //    UserId = UserDAO.FindAll().FirstOrDefault(x => x.Email == Model.Email)?.Id,
                    //    Name = Model.Name,
                    //    Cpf = Model.Cpf
                    //});

                    #endregion

                    #region Saving User and Client

                    var user = new User()
                    {
                        Email = Model.Email,
                        Type = Model.Type,
                        Password = Encrypt.EncryptPass(Model.Password)
                    };

                    var client = new Client()
                    {
                        UserId = UserDAO.FindAll().FirstOrDefault(x => x.Email == Model.Email)?.Id,
                        Name = Model.Name,
                        Cpf = Model.Cpf
                    };

                    UserDAO.SaveNewUser<User, Client>(user, client);

                    return new APIResult<string>()
                    {
                        Message = "Cliente cadastrado com sucesso!"
                    };

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