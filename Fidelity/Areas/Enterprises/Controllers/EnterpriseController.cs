﻿using Fidelity.Areas.Enterprises.Models;
using Fidelity.Areas.Users.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity;
using FidelityLibrary.Entity.Users;
using FidelityLibrary.Models;
using FidelityLibrary.Persistance.EmployeeDAO;
using FidelityLibrary.Persistance.EnterpriseDAO;
using FidelityLibrary.Persistance.UserDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Enterprises.Controllers
{
    public class EnterpriseController : ApiController
    {
        /// <summary>
        /// Requisição para cadastrar empresa no sistema.
        /// </summary>
        /// <param name="Model"></param>
        /// <returns>API Result Object</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("enterprises")]
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
                                    Email = Model.Email.ToLower(),
                                    Type = "E",
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
                                    Active = true
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
        /// Requisição para buscar empresas participantes no sistema.
        /// </summary>
        /// <returns>APIResult List Object></returns>
        [HttpGet]
        [Authorize]
        [Route("enterprises")]
        public APIResult<List<EnterpriseViewModel>> Get()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var oEnterpriseList = new List<EnterpriseViewModel>();
                    foreach (var item in EnterpriseDAO.FindAll().ToList())
                    {
                        oEnterpriseList.Add(new EnterpriseViewModel()
                        {
                            Name = item.Name,
                            AlterDate = item.AlterDate,
                            Active = item.Active,
                            Address = item.Address,
                            AddressNum = item.AddressNum,
                            Branch = item.Branch,
                            City = item.City,
                            Cnpj = item.Cnpj,
                            MembershipId = item.MembershipId,
                            State = item.State,
                            Tel = item.Tel
                        });
                    }

                    return new APIResult<List<EnterpriseViewModel>>()
                    {
                        Result = oEnterpriseList,
                        Count = oEnterpriseList.Count
                    };
                }
                else
                    return new APIResult<List<EnterpriseViewModel>>()
                    {
                        Success = false,
                        Message = "Acesso negado!"
                    };
            }
            catch (Exception e)
            {
                return new APIResult<List<EnterpriseViewModel>>()
                {
                    Success = false,
                    Message = "Erro ao buscar as empresas participantes: " + e.Message + e.InnerException
                };
            }
        }
    }
}