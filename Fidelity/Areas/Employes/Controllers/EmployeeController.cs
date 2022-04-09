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
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Employes.Controllers
{
    public class EmployeeController : ApiController
    {
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
                                    Active = "1",
                                    Password = Encrypt.EncryptPass(Model.Password)
                                };

                                UserDAO.SaveUser(user, context);

                                var oEmploye = new Employee()
                                {
                                    UserId = user.Id,
                                    EnterpriseId = Model.Employee.EnterpriseId,
                                    Name = Model.Employee.Name,
                                    AccessType = Model.Employee.AccessType
                                };

                                EmployeeDAO.SaveEmployee(oEmploye, context);

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

        [HttpGet]
        [Authorize]
        [Route("employees")]
        public APIResult<List<EmployeeViewModel>> Get()
        {
            try
            {
                #region GET PARAMS

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                foreach (var parameter in Request.GetQueryNameValuePairs())
                {
                    parameters.Add(parameter.Key, parameter.Value);
                }

                var company = 0;

                if (parameters.ContainsKey("company"))
                {
                    company = Int32.Parse(parameters["company"]);
                }
                else
                {
                    return new APIResult<List<EmployeeViewModel>>()
                    {
                        Success = false,
                        Message = "Nenhuma empresa informada!"
                    };
                }

                #endregion

                var oEmployeeList = new List<EmployeeViewModel>();

                foreach (var item in EmployeeDAO.FindAll().Where(x => x.EnterpriseId == company).ToList())
                {
                    oEmployeeList.Add(new EmployeeViewModel()
                    {
                        Id = item.Id,
                        AccessType = item.AccessType,
                        EnterpriseId = item.EnterpriseId,
                        Name = item.Name
                    });
                }

                return new APIResult<List<EmployeeViewModel>>()
                {
                    Result = oEmployeeList,
                    Count = oEmployeeList.Count
                };

            }
            catch (Exception e)
            {
                return new APIResult<List<EmployeeViewModel>>()
                {
                    Success = false,
                    Message = "Erro ao buscar todos funcionários: " + e.Message + e.InnerException
                };
            }
        }

        [HttpPut]
        [Authorize]
        [Route("employees")]
        public APIResult<Object> Update(EmployeeViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var oEmployee = EmployeeDAO.FindByKey(Model.Id);

                    oEmployee.Name = Model.Name;
                    oEmployee.AccessType = Model.AccessType;
                    oEmployee.AlterDate = DateTime.Now;

                    EmployeeDAO.Update(oEmployee);

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

        [HttpDelete]
        [Authorize]
        [Route("employees")]
        public APIResult<Object> Delete(EmployeeViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var oEmployee = EmployeeDAO.FindByKey(Model.Id);

                    EmployeeDAO.Delete(oEmployee);

                    return new APIResult<object>()
                    {
                        Message = "Funcionário deletado com sucesso!"

                        // deletar UserId também ou mudar tag para inativo invés de deletar funcionário??
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