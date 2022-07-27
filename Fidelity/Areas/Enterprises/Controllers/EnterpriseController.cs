using Fidelity.Areas.Enterprises.Models;
using Fidelity.Areas.Loyalts.Models;
using Fidelity.Areas.Users.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity;
using FidelityLibrary.Entity.Users;
using FidelityLibrary.Models;
using FidelityLibrary.Persistance.EmployeeDAO;
using FidelityLibrary.Persistance.EnterpriseDAO;
using FidelityLibrary.Persistance.LoyaltyDAO;
using FidelityLibrary.Persistance.UserDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                                    Status = true,
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
                                    Cnpj = Model.Enterprise.Cnpj.Replace(".", "").Replace("/", "").Replace("-", ""),
                                    MembershipId = Model.Enterprise.MembershipId,
                                    State = Model.Enterprise.State,
                                    Tel = Model.Enterprise.Tel
                                };

                                EnterpriseDAO.SaveEnterprise(Enterprise, context);

                                #endregion

                                #region Employee

                                var Employee = new Employee()
                                {
                                    UserId = user.Id,
                                    EnterpriseId = Enterprise.Id,
                                    AccessType = 0,
                                    Name = Model.Enterprise.EmployeeName
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
        public APIResult<List<UserViewModel>> Get()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    #region GET PARAMS

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    foreach (var parameter in Request.GetQueryNameValuePairs())
                    {
                        parameters.Add(parameter.Key, parameter.Value);
                    }

                    var Id = 0;

                    if (parameters.ContainsKey("id"))
                    {
                        Id = Convert.ToInt32(parameters["id"]);
                    }

                    #endregion


                    var EnterpriseList = new List<EnterpriseViewModel>();
                    var UserList = new List<UserViewModel>();

                    if (Id != 0)
                    {
                        var LoyaltList = new List<LoyaltViewModel>();
                        var Enterprise = EnterpriseDAO.FindByKey(Id);
                        var LoyaltsDB = LoyaltyDAO.FindAll().Where(x => x.EnterpriseId == Id && x.Status).ToList();
                        foreach (var item in LoyaltsDB)
                        {
                            LoyaltList.Add(new LoyaltViewModel()
                            {
                                Id = item.Id,
                                EnterpriseId = item.EnterpriseId,
                                CouponValue = item.CouponValue,
                                Description = item.Description,
                                EndDate = item.EndDate,
                                FidelityTypeId = item.FidelityTypeId,
                                Limit = item.Limit,
                                Name = item.Name,
                                ProductId = item.ProductId,
                                PromotionTypeId = item.PromotionTypeId,
                                Quantity = item.Quantity,
                                StartDate = item.StartDate
                            });
                        }

                        var User = UserDAO.FindAll().FirstOrDefault(x => x.Id == Enterprise.UserId);

                        var EnterpriseVM = new EnterpriseViewModel()
                        {
                            Id = Enterprise.Id,
                            UserId = Enterprise.UserId,
                            Name = Enterprise.Name,
                            AlterDate = Enterprise.AlterDate,
                            Status = User.Status,
                            Address = Enterprise.Address,
                            AddressNum = Enterprise.AddressNum,
                            Branch = Enterprise.Branch,
                            City = Enterprise.City,
                            Cnpj = Enterprise.Cnpj,
                            MembershipId = Enterprise.MembershipId,
                            State = Enterprise.State,
                            Tel = Enterprise.Tel,
                            Loyalts = LoyaltList
                        };

                        UserList.Add(new UserViewModel
                        {
                            Image = User.Image,
                            Status = User.Status,
                            Email = User.Email,
                            Type = User.Type,
                            Enterprise = EnterpriseVM,
                        });

                        return new APIResult<List<UserViewModel>>()
                        {
                            Result = UserList,
                            Count = UserList.Count
                        };
                    }

                    foreach (var item in EnterpriseDAO.FindAll().Join(UserDAO.FindAll(), e => e.UserId, u => u.Id, (e, u) => new { E = e, U = u }).Where(EU => EU.E.UserId == EU.U.Id && EU.U.Status).ToList())
                    {
                        var EnterpriseUser = UserDAO.FindAll().FirstOrDefault(x => x.Id == item.U.Id);

                        var EnterpriseVM = new EnterpriseViewModel()
                        {
                            Name = item.E.Name,
                            AlterDate = item.E.AlterDate,
                            Status = item.U.Status,
                            Address = item.E.Address,
                            AddressNum = item.E.AddressNum,
                            Branch = item.E.Branch,
                            City = item.E.City,
                            Cnpj = item.E.Cnpj,
                            MembershipId = item.E.MembershipId,
                            State = item.E.State,
                            Tel = item.E.Tel,
                            Id = item.E.Id,
                            UserId = item.U.Id,
                        };

                        UserList.Add(new UserViewModel
                        {
                            Image = EnterpriseUser.Image,
                            Status = EnterpriseUser.Status,
                            Email = EnterpriseUser.Email,
                            Type = EnterpriseUser.Type,
                            Enterprise = EnterpriseVM,
                        });
                    }

                    return new APIResult<List<UserViewModel>>()
                    {
                        Result = UserList,
                        Count = UserList.Count
                    };
                }
                else
                    return new APIResult<List<UserViewModel>>()
                    {
                        Success = false,
                        Message = "Acesso negado!"
                    };
            }
            catch (Exception e)
            {
                return new APIResult<List<UserViewModel>>()
                {
                    Success = false,
                    Message = "Erro ao buscar as empresas participantes: " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para atualizar empresa no sistema.
        /// </summary>
        /// <returns>Enterprise List Object></returns>
        [HttpPut]
        [Authorize]
        [Route("enterprises")]
        public APIResult<Object> Update(UserViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var User = UserDAO.FindByKey(Model.Enterprise.UserId);

                    if (Model.Password != null)
                    {
                        User.Password = Encrypt.EncryptPass(Model.Password);
                    }

                    User.Email = Model.Email;
                    User.Image = Model.Image;
                    User.Status = Model.Status;
                    User.AlterDate = DateTime.Now;

                    var Company = EnterpriseDAO.FindByKey(Model.Enterprise.Id);

                    Company.Name = Model.Enterprise.Name ?? "";
                    Company.Address = Model.Enterprise.Address ?? "";
                    Company.AddressNum = Model.Enterprise.AddressNum ?? "";
                    Company.Branch = Model.Enterprise.Branch ?? "";
                    Company.City = Model.Enterprise.City ?? "";
                    Company.Cnpj = Model.Enterprise.Cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
                    Company.MembershipId = Model.Enterprise.MembershipId ?? 1;
                    Company.State = Model.Enterprise.State ?? "";
                    Company.Tel = Model.Enterprise.Tel ?? "";

                    EnterpriseDAO.Update(Company);
                    UserDAO.Update(User);

                    return new APIResult<object>()
                    {
                        Message = "Empresa atualizada com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao atualizar empresa: " + e.Message + e.InnerException
                };
            }
        }
    }
}