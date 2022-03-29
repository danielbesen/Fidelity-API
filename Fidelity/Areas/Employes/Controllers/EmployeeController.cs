using Fidelity.Areas.Employes.Models;
using Fidelity.Models;
using FidelityLibrary.Persistance.ClientDAO;
using FidelityLibrary.Persistance.EmployeeDAO;
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
        public APIResult<Object> Salvar(EmployeeViewModel Model)
        {
            try
            {
                if (EmployeeDAO.FindAll().ToList().Any())
                {

                }

                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "E-mail já cadastrado!",
                }; ;
                
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
                if (EmployeeDAO.FindAll().ToList().Any())
                {

                }

                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "E-mail já cadastrado!",
                }; ;

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
                    Success = false,
                    Message = "E-mail já cadastrado!",
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
                if (EmployeeDAO.FindAll().ToList().Any())
                {

                }

                return new APIResult<Object>()
                {
                    Success = false,
                    Message = "E-mail já cadastrado!",
                }; ;

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