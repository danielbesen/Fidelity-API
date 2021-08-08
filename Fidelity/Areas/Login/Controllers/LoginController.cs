﻿using Fidelity.Areas.Clients.Models;
using Fidelity.Models;
using FidelityLibrary.Entity;
using FidelityLibrary.Entity.Users;
using FidelityLibrary.Models;
using FidelityLibrary.Persistance.ClientDAO;
using FidelityLibrary.Persistance.EmployeeDAO;
using FidelityLibrary.Persistance.EnterpriseDAO;
using FidelityLibrary.Persistance.UserDAO;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Login.Controllers
{
    public class LoginController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public APIResult<Object> Login(User oUser)
        {
            try
            {
                if (UserDAO.FindAll().ToList().Any(x => x.Name == oUser.Name && Encrypt.VerifyPass(oUser.Password, x.Password)))
                {
                    oUser = UserDAO.GetUser(oUser);

                    return new APIResult<Object>()
                    {
                        Object = new LoginResult<Object>()
                        {
                            Token = Encrypt.GetToken(oUser.Name, oUser.Password),
                            Property = oUser.Type == "C" ? ClientDAO.FindByUserId(oUser.Id) : oUser.Type == "E" ? EnterpriseDAO.FindByUserId(oUser.Id) : EmployeeDAO.FindByUserId(oUser.Id),
                            Type = oUser.Type.ToString()
                        },
                        Message = "Usuário logado com sucesso!"
                    };
                }
                else if (UserDAO.FindAll().Any(x => x.Name == oUser.Name))
                {
                    return new APIResult<Object>()
                    {
                        Success = false,
                        Message = "Senha incorreta!"
                    };
                }
                else
                {
                    return new APIResult<Object>()
                    {
                        Success = false,
                        Message = "Usuário não cadastrado!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao validar Login: " + e.Message,
                };
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("signup")]
        public APIResult<string> Signup(ClientViewModel Model)
        {
            try
            {
                if (UserDAO.FindAll().ToList().Any(x => x.Name == Model.Email))
                {
                    return new APIResult<string>()
                    {
                        Success = false,
                        Message = "E-mail já cadastrado!",
                    };
                }
                else
                {
                    //fazer uma transaction aqui pra caso 1 insert der falha, voltar tudo
                    UserDAO.Insert(new User()
                    {
                        Name = Model.Email,
                        Type = Model.Type,
                        Password = Encrypt.EncryptPass(Model.Password)
                    });

                    ClientDAO.Insert(new Client()
                    {
                        UserId = UserDAO.FindAll().FirstOrDefault(x => x.Name == Model.Email).Id,
                        Name = Model.Name,
                        Cpf = Model.Cpf
                    });

                    return new APIResult<string>()
                    {
                        Message = "Cliente cadastrado com sucesso!"
                    };
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