using Fidelity.Areas.Clients.Models;
using Fidelity.Areas.Users.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
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
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Users.Controllers
{
    public class UserController : ApiController
    {
        /// <summary>
        /// Requisição para redefinição de senha
        /// </summary>>
        /// <param name="Email"></param>
        /// <returns>API Result Object</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("reset/pass")]
        public APIResult<Object> ResetPassword([FromBody] string Email)
        {
            try
            {
                if (!UserDAO.FindAll().Any(x => x.Email == Email))
                {
                    return new APIResult<Object>()
                    {
                        Success = false,
                        Message = "Nenhum registro com esse e-mail encontrado!"
                    };
                }
                else
                {
                    var oUser = UserDAO.GetUser(Email);

                    var Password = CreatePassword(8);
                    oUser.Password = Encrypt.EncryptPass(Password);

                    using (var context = new ApplicationDbContext())
                    {
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            UserDAO.UpdateUser(oUser, context);

                            #region Envio de e-mail

                            MailMessage mail = new MailMessage();

                            mail.From = new MailAddress("de@gmail.com");
                            mail.To.Add("para@gmail.com");
                            mail.Subject = "Teste";
                            mail.Body = "Sua nova senha é " + Password;

                            using (var smtp = new SmtpClient("smtp.gmail.com"))
                            {
                                smtp.EnableSsl = true;
                                smtp.Port = 587;
                                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = new NetworkCredential("@gmail.com", "");
                                smtp.Send(mail);
                            }

                            #endregion

                            dbContextTransaction.Commit();

                            return new APIResult<Object>()
                            {
                                Message = "E-mail enviado com sucesso!"
                            };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao resetar senha! " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Função para gerar senha aleatória
        /// </summary>>
        /// <param name="length"></param>
        /// <returns>String</returns>
        public string CreatePassword(int length)
        {
            try
            {
                const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                StringBuilder res = new StringBuilder();
                Random random = new Random();
                while (0 < length--)
                {
                    res.Append(valid[random.Next(valid.Length)]);
                }
                return res.ToString();
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao gerar nova senha: " + e.Message + e.InnerException);
            }

        }
    }
}