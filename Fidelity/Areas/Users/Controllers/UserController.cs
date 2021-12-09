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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fidelity.Areas.Users.Controllers
{
    public class UserController : Controller
    {
        /// <summary>
        /// Requisição para cadastrar cliente no sistema.
        /// </summary>
        /// <returns>API Result Object</returns>
        /// <param name="Model"></param>
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
                        throw new Exception("Transaction insert error: " + e);
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
        /// Requisição para Empresa cliente no sistema.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns>API Result Object</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("signup/enterprise")]
        public APIResult<string> SignupEnterprise(UserViewModel Model)
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
                                    Name = Model.Enteprise.Name,
                                    Address = Model.Enteprise.Address,
                                    AddressNum = Model.Enteprise.AddressNum,
                                    Branch = Model.Enteprise.Branch,
                                    City = Model.Enteprise.City,
                                    Cnpj = Model.Enteprise.Cnpj,
                                    MembershipId = Model.Enteprise.MembershipId,
                                    State = Model.Enteprise.State,
                                    Tel = Model.Enteprise.Tel,
                                    Active = "1"
                                };

                                EnterpriseDAO.SaveEnterprise(Enterprise, context);

                                #endregion

                                #region Employee

                                var Employee = new Employee()
                                {
                                    EnterpriseId = Enterprise.Id,
                                    AccessType = 0
                                };

                                EmployeeDAO.SaveEmployee(Employee, context);

                                #endregion

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