using Fidelity.Areas.Clients.Models;
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
        public APIResult<Object> Login(User oUser)
        {
            try
            {
                if (UserDAO.FindAll().ToList().Any(x => x.Email == oUser.Email && Encrypt.VerifyPass(oUser.Password, x.Password)))
                {
                    var oNewUser = UserDAO.GetUser(oUser);

                    return new APIResult<Object>()
                    {
                        Result = new LoginResult<Object>()
                        {
                            Token = Encrypt.GetToken(oUser.Email, oUser.Password),
                            Property = oUser.Type == "C" ? ClientDAO.FindByUserId(oNewUser.Id) : oUser.Type == "E" ? EnterpriseDAO.FindByUserId(oNewUser.Id) : EmployeeDAO.FindByUserId(oNewUser.Id),
                            Type = oUser.Type.ToString()
                        },
                        Message = "Usuário logado com sucesso!"
                    };
                }
                else if (UserDAO.FindAll().Any(x => x.Email == oUser.Email))
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