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
        public APIResult<List<ClientViewModel>> GetClients()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var oClientList = new List<ClientViewModel>();
                    foreach (var item in ClientDAO.FindAll().ToList())
                    {
                        oClientList.Add(new ClientViewModel()
                        {
                            Cpf = item.Cpf,
                            Name = item.Name
                        });
                    }

                    return new APIResult<List<ClientViewModel>>()
                    {
                        Result = oClientList,
                        Count = ClientDAO.FindAll().ToList().Count
                    };
                }
                else
                    return new APIResult<List<ClientViewModel>>()
                    {
                        Success = false,
                        Message = "Acesso negado!"
                    };
            }
            catch (Exception e)
            {
                return new APIResult<List<ClientViewModel>>()
                {
                    Success = false,
                    Message = "Erro ao buscar todos clientes: " + e.Message,
                };
            }
        }
    }
}