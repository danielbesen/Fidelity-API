using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity;
using FidelityLibrary.Persistance.ClientDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Security.Claims;
using Fidelity.Areas.Clients.Models;
using FidelityLibrary.Persistance.UserDAO;
using FidelityLibrary.Entity.Users;
using FidelityLibrary.Models;
using FidelityLibrary.Persistance.Generics;
using Fidelity.Areas.Users.Models;
using Fidelity.Areas.Enterprises.Models;

namespace Fidelity.Areas.Clients.Controllers
{
    public class ClientController : ApiController
    {
        /// <summary>
        /// Requisição para buscar todos os clientes no sistema.
        /// </summary>
        /// <returns>Client List Object></returns>
        [HttpGet]
        [Authorize]
        [Route("clients")]
        public APIResult<List<Client>> Get()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return new APIResult<List<Client>>()
                    {
                        Result = ClientDAO.FindAll().ToList(),
                        Count = ClientDAO.FindAll().ToList().Count
                    };
                }
                else
                    return new APIResult<List<Client>>()
                    {
                        Success = false,
                        Message = "Acesso negado!"
                    };
            }
            catch (Exception e)
            {
                return new APIResult<List<Client>>()
                {
                    Success = false,
                    Message = "Erro ao buscar todos clientes: " + e.Message,
                };
            }
        }
    }
}