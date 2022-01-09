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
        /// Requisição para cadastrar cliente no sistema.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns>API Result Object</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("signup/client")]
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
                                    Name = Model.Name,
                                    Email = Model.Email,
                                    Type = Model.Type,
                                    Active = "1",
                                    Password = Encrypt.EncryptPass(Model.Password)
                                };

                                UserDAO.SaveUser(user, context);

                                var client = new Client()
                                {
                                    UserId = user.Id,//UserDAO.FindAll().FirstOrDefault(x => x.Email == Model.Email)?.Id,
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
                            Message = "Erro na transação: " + e.Message,
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
                    Message = "Erro ao validar Login: " + e.Message,
                };
            }
        }

        /// <summary>
        /// Requisição para cadastrar empresa no sistema.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns>API Result Object</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("signup/enterprise")]
        public APIResult<Object> SignupEnterprise(UserViewModel Model)
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
                    #region Saving User and Enterprise

                    try
                    {
                        using (var context = new ApplicationDbContext())
                        {
                            using (var dbContextTransaction = context.Database.BeginTransaction())
                            {
                                #region User

                                var user = new User()
                                {
                                    Name = Model.Name,
                                    Email = Model.Email,
                                    Type = Model.Type,
                                    Active = "1",
                                    Password = Encrypt.EncryptPass(Model.Password)
                                };

                                UserDAO.SaveUser(user, context);

                                #endregion

                                #region Enterprise

                                var Enterprise = new Enterprise()
                                {
                                    UserId = user.Id,
                                    Name = Model.Enterprise.Name,
                                    Address = Model.Enterprise.Address,
                                    AddressNum = Model.Enterprise.AddressNum,
                                    Branch = Model.Enterprise.Branch,
                                    City = Model.Enterprise.City,
                                    Cnpj = Model.Enterprise.Cnpj,
                                    MembershipId = Model.Enterprise.MembershipId,
                                    State = Model.Enterprise.State,
                                    Tel = Model.Enterprise.Tel,
                                    Active = "1"
                                };

                                EnterpriseDAO.SaveEnterprise(Enterprise, context);

                                #endregion

                                #region Employee

                                var Employee = new Employee()
                                {
                                    UserId = user.Id,
                                    EnterpriseId = Enterprise.Id,
                                    AccessType = 0
                                };

                                EmployeeDAO.SaveEmployee(Employee, context);

                                #endregion

                                dbContextTransaction.Commit();

                                return new APIResult<Object>()
                                {
                                    Message = "Empresa cadastrada com sucesso!"
                                };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        return new APIResult<Object>()
                        {
                            Success = false,
                            Message = "Erro na transação: " + e.Message,
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
                    Message = "Erro ao validar Login: " + e.Message,
                };
            }
        }

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
                            mail.Body = "Testando mensagem de e-mail";

                            using (var smtp = new SmtpClient("smtp.gmail.com"))
                            {
                                smtp.EnableSsl = true;
                                smtp.Port = 587;
                                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = new NetworkCredential("fidelity@gmail.com", "sua senha");
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
                    Message = "Erro ao resetar senha! " + e.Message
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
                throw new Exception("Erro ao gerar nova senha: " + e.Message);
            }

        }
    }
}