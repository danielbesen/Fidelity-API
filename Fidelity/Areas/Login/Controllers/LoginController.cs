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
                    var oNewUser = UserDAO.GetUser(Model.Email);

                    return new APIResult<Object>()
                    {
                        Result = new LoginResult<Object>()
                        {
                            Token = Encrypt.GetToken(Model.Email, Model.Password),
                            Property = oNewUser.Type == "C" ? ClientDAO.FindByUserId(oNewUser.Id) : oNewUser.Type == "E" ? EnterpriseDAO.FindByUserId(oNewUser.Id) : EmployeeDAO.FindByUserId(oNewUser.Id),
                            Type = oNewUser.Type.ToString()
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
                    Message = "Erro ao validar Login: " + e.Message,
                };
            }
        }
    }
}