﻿using Fidelity.Models;
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
                    foreach (var item in ClientDAO.FindAll().ToList())
                    {
                        oClientList.Add(new ClientViewModel()
                        {
                            Id = item.Id,
                            Cpf = item.Cpf,
                            Name = item.Name
                        });
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
        public APIResult<Object> Signup(UserViewModel Model)
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

        /// <summary>
        /// Requisição para buscar cliente por Documento.
        /// </summary>
        /// <returns>Client List Object></returns>
        [HttpGet]
        [Authorize]
        [Route("clients/document")]
        public APIResult<ClientViewModel> GetByDocument(ClientViewModel Model)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var oClient = ClientDAO.FindAll().FirstOrDefault(x => x.Cpf == Model.Cpf);

                    if (oClient != null)
                    {
                        var Client = new ClientViewModel
                        {
                            Id = oClient.Id,
                            Name = oClient.Name,
                            Cpf = oClient.Cpf
                        };

                        return new APIResult<ClientViewModel>()
                        {
                            Result = Client
                        };
                    }
                    else {
                        return new APIResult<ClientViewModel>()
                        {
                            Success = false,
                            Message = "Nenhum cliente com esse CPF encontrado!"
                        };
                    }
                }
                else
                    return new APIResult<ClientViewModel>()
                    {
                        Success = false,
                        Message = "Acesso negado!"
                    };
            }
            catch (Exception e)
            {
                return new APIResult<ClientViewModel>()
                {
                    Success = false,
                    Message = "Erro ao buscar todos clientes: " + e.Message + e.InnerException
                };
            }
        }
    }
}