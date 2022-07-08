using Fidelity.Areas.Employes.Models;
using Fidelity.Areas.Enterprises.Models;
using Fidelity.Areas.Users.Models;
using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity;
using FidelityLibrary.Entity.Users;
using FidelityLibrary.Models;
using FidelityLibrary.Persistance.ClientDAO;
using FidelityLibrary.Persistance.EmployeeDAO;
using FidelityLibrary.Persistance.UserDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Employes.Controllers
{
    public class EmployeeController : ApiController
    {
        /// <summary>
        /// Requisição para cadastrar funcionário no sistema.
        /// </summary>
        /// <returns>Product List Object></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("employees")]
        public APIResult<Object> SignUp(UserViewModel Model)
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
                    #region Saving User and Employee
                    try
                    {
                        using (var context = new ApplicationDbContext())
                        {
                            using (var dbContextTransaction = context.Database.BeginTransaction())
                            {
                                var user = new User()
                                {
                                    Email = Model.Email.ToLower(),
                                    Type = "F",
                                    Status = true,
                                    Password = Encrypt.EncryptPass(Model.Password),
                                    Image = Model.Image,
                                };

                                UserDAO.SaveUser(user, context);

                                var Employee = new Employee()
                                {
                                    UserId = user.Id,
                                    EnterpriseId = Model.Employee.EnterpriseId,
                                    Name = Model.Employee.Name,
                                    AccessType = 1
                                };

                                EmployeeDAO.SaveEmployee(Employee, context);

                                dbContextTransaction.Commit();

                                return new APIResult<Object>()
                                {
                                    Success = true,
                                    Message = "Funcionário cadastrado com sucesso!",
                                };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        return new APIResult<Object>()
                        {
                            Success = false,
                            Message = "Erro na transação: " + e.Message + e.InnerException,
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
                    Message = "Erro ao Salvar Funcionario: " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para buscar funcionário no sistema.
        /// </summary>
        /// <returns>Product List Object></returns>
        [HttpGet]
        [Authorize]
        [Route("employees")]
        public APIResult<object> Get()
        {
            try
            {
                var company = 0;
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    company = Convert.ToInt32(identity.FindFirst("company").Value);
                }

                #region GET PARAMS

                //Dictionary<string, string> parameters = new Dictionary<string, string>();
                //foreach (var parameter in Request.GetQueryNameValuePairs())
                //{
                //    parameters.Add(parameter.Key, parameter.Value);
                //}

                //var company = 0;

                //if (parameters.ContainsKey("company"))
                //{
                //    company = Int32.Parse(parameters["company"]);
                //}
                //else
                //{
                //    return new APIResult<object>()
                //    {
                //        Success = false,
                //        Message = "Nenhuma empresa informada!"
                //    };
                //}

                #endregion

                var UserList = new List<UserViewModel>();

                foreach (var item in EmployeeDAO.FindAll().Join(UserDAO.FindAll(), e => e.UserId, u => u.Id, (e, u) => new { E = e, U = u }).Where(EU => EU.E.UserId == EU.U.Id && EU.U.Status && EU.E.EnterpriseId == company).ToList())
                {
                    var Employee = new EmployeeViewModel()
                    {
                        Id = item.E.Id,
                        UserId = item.E.UserId,
                        AccessType = item.E.AccessType,
                        EnterpriseId = item.E.EnterpriseId,
                        Name = item.E.Name
                    };

                    var oUser = UserDAO.FindByKey(item.E.UserId);

                    UserList.Add(new UserViewModel()
                    {
                        Image = oUser.Image,
                        Status = oUser.Status,
                        Email = oUser.Email,
                        Type = oUser.Type,
                        Employee = Employee,
                    });
                }

                return new APIResult<object>()
                {
                    Result = UserList,
                    Count = UserList.Count
                };

            }
            catch (Exception e)
            {
                return new APIResult<object>()
                {
                    Success = false,
                    Message = "Erro ao buscar todos funcionários: " + e.Message + e.InnerException
                };
            }
        }


        /// <summary>
        /// Requisição para atualizar funcionário no sistema.
        /// </summary>
        /// <returns>Product List Object></returns>
        [HttpPut]
        [Authorize]
        [Route("employees")]
        public APIResult<Object> Update(UserViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var User = UserDAO.FindByKey(Model.Employee.UserId);

                    if (Model.Password != null)
                    {
                        User.Password = Encrypt.EncryptPass(Model.Password);
                    }

                    User.Email = Model.Email;
                    User.Image = Model.Image;
                    User.Status = Model.Status;
                    User.AlterDate = DateTime.Now;

                    var Employee = EmployeeDAO.FindByKey(Model.Employee.Id);
                    Employee.Name = Model.Employee.Name;
                    Employee.AccessType = Model.Employee.AccessType;
                    Employee.AlterDate = DateTime.Now;

                    EmployeeDAO.Update(Employee);
                    UserDAO.Update(User);

                    return new APIResult<object>()
                    {
                        Message = "Funcionário atualizado com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao atualizar funcionário: " + e.Message + e.InnerException
                };
            }
        }

        /// <summary>
        /// Requisição para deletar um funcionário no sistema.
        /// </summary>
        /// <returns>Product List Object></returns>
        [HttpDelete]
        [Authorize]
        [Route("employees")]
        public APIResult<Object> Delete(EmployeeViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var Employee = EmployeeDAO.FindByKey(Model.Id);

                    var User = UserDAO.FindByKey(Employee.UserId);
                    User.Status = false;
                    EmployeeDAO.Update(Employee);

                    return new APIResult<object>()
                    {
                        Message = "Funcionário deletado com sucesso!"
                    };
                }
            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao deletar Funcionario: " + e.Message + e.InnerException
                };
            }
        }

    }
}