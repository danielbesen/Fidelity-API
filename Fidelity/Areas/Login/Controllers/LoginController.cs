using Fidelity.Models;
using FidelityLibrary.Entity;
using FidelityLibrary.Entity.Users;
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
using System.Web.Mvc;

namespace Fidelity.Areas.Login.Controllers
{
    public class LoginController : ApiController
    {
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("login")]
        public APIResult<Object> Get(User oUser)
        {
            try
            {
                if (UserDAO.FindAll().ToList().Any(x => x.Name == oUser.Name && x.Password == oUser.Password))
                {
                    oUser = UserDAO.GetUser(oUser);

                    return new APIResult<Object>()
                    {
                        Object = new LoginResult<Object>() {
                            Token = GetToken(oUser.Name, oUser.Password),
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

        public Object GetToken(string Username, string Password)
        {
            string key = "my_secret_key_12345"; //Secret key which will be used later during validation    
            var issuer = "https://localhost:44387";  //normally this will be your site URL    

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Create a List of Claims, Keep claims name short    
            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim("username", Username));
            permClaims.Add(new Claim("password", Password));

            //Create Security Token object by giving required parameters    
            var token = new JwtSecurityToken(issuer, //Issure    
                            issuer,  //Audience    
                            permClaims,
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: credentials);
            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
            return new { data = jwt_token };
        }
    }
}