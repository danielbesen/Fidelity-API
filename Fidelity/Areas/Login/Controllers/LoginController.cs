using Fidelity.Areas.Clients.Models;
using Fidelity.Areas.Users.Models;
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
        /// <summary>
        /// Requisição para logar o usuário no sistema.
        /// </summary>
        /// <param name="oUser"></param>
        /// <returns>API Result Object</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public APIResult<Object> Login(UserViewModel Model)
        {
            try
            {
                if (UserDAO.FindAll().ToList().Any(x => x.Email.ToLower() == Model.Email.ToLower() && Encrypt.VerifyPass(Model.Password, x.Password)))
                {
                    var NewUser = UserDAO.GetUser(Model.Email);

                    int EnterpriseId = NewUser.Type == "E" ? EnterpriseDAO.FindByUserId(NewUser.Id).Id : NewUser.Type == "F" ? EmployeeDAO.FindByUserId(NewUser.Id).EnterpriseId : -1;
                    object Token = null;

                    if (EnterpriseId != -1)
                    {
                        Token = Encrypt.GetTokenCompany(Model.Email, Model.Password, EnterpriseId);
                    } else
                    {
                        Token = Encrypt.GetToken(Model.Email, Model.Password);
                    }

                    return new APIResult<Object>()
                    {
                        Result = new LoginResult<Object>()
                        {
                            Token = Token,
                            Property = NewUser.Type == "C" ? ClientDAO.FindByUserId(NewUser.Id) : NewUser.Type == "E" ? EnterpriseDAO.FindByUserId(NewUser.Id) : EmployeeDAO.FindByUserId(NewUser.Id),
                            Image = NewUser.Image,
                            Type = NewUser.Type.ToString()
                        },
                        Message = "Usuário logado com sucesso!"
                    };
                }
                else if (UserDAO.FindAll().Any(x => x.Email == Model.Email))
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
                    Message = "Erro ao validar Login: " + e.Message + e.InnerException
                };
            }
        }
    }
}