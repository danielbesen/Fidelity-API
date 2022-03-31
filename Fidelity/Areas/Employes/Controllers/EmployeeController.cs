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
using System.Web;
using System.Web.Http;

namespace Fidelity.Areas.Employes.Controllers
{
    public class EmployeeController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("enploye")]
        public APIResult<Object> Salvar(EmployeeViewModel Model, UserViewModel UserModel)
        {
            try
            {
                if ((UserDAO.FindAll().ToList().Any(x => x.Email == UserModel.Email && x.Type == UserModel.Type)) && (EmployeeDAO.FindAll().ToList().Any(x => x.Id == Model.Id && x.EnterpriseId == Model.EnterpriseId)))
                {
                    return new APIResult<Object>()
                    {
                        Success = false,
                        Message = "Funcionário já cadastrado!",
                    };
                }
                else
                {
                    #region Saving User and Employe
                    try
                    {
                        using (var context = new ApplicationDbContext())
                        {
                            using (var dbContextTransaction = context.Database.BeginTransaction())
                            {
                                var user = new User()
                                {
                                    Email = UserModel.Email.ToLower(),
                                    Type = "F",
                                    Active = "1",
                                    Password = Encrypt.EncryptPass(UserModel.Password)
                                };

                                UserDAO.SaveUser(user, context);

                                var oEmploye = new Employee()
                                {
                                    Id = Model.Id,
                                    UserId = user.Id,
                                    EnterpriseId = Model.EnterpriseId,
                                    Nome = Model.Name,
                                    AccessType = Model.AccessType,
                                    InsertDate = DateTime.Now
                                };

                                EmployeeDAO.SaveEmployee(oEmploye, context);

                                dbContextTransaction.Commit();

                                return new APIResult<Object>()
                                {
                                    Success = true,
                                    Message = "Funcionário cadastrado com sucesso",
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
        [AllowAnonymous]
        [Route("enploye")]
        public APIResult<Object> Buscar(EmployeeViewModel Model)
        {
            try
            {
                //

                var oEmploy = EmployeeDAO.FindAll().FirstOrDefault(x => x.Id == Model.Id);

                if (oEmploy != null)
                {
                    var oEmploye = new EmployeeViewModel
                    {
                        Id = Model.Id,
                        AccessType = Model.AccessType
                    };

                    return new APIResult<Object>()
                    {
                        Result = oEmploye
                    };

                }

                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao buscar Funcionario"
                };

            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao buscar Funcionario: " + e.Message + e.InnerException
                };
            }
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("enploye")]
        public APIResult<Object> Atualizar(EmployeeViewModel Model)
        {
            try
            {
                if (EmployeeDAO.FindAll().ToList().Any())
                {

                }

                return new APIResult<Object>()
                {
                    Success = true,
                    Message = "Funcionário atualizado com sucesso!"
                }; ;

            }
            catch (Exception e)
            {
                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "Erro ao atualizar Funcionario: " + e.Message + e.InnerException
                };
            }
        }

        [HttpDelete]
        [AllowAnonymous]
        [Route("enploye")]
        public APIResult<Object> Delete(EmployeeViewModel Model)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var oEmploye = new Employee
                    {
                        Id = Model.Id,
                        AlterDate = DateTime.Now,
                    };

                    EmployeeDAO.DelEmployee(oEmploye, context);

                    return new APIResult<object>()
                    {
                        Success = true,
                        Message = "Funcionário deletada com sucesso!"
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