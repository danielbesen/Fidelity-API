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

namespace Fidelity.Areas.Clients.Controllers
{
    public class ClientController : ApiController
    {
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

        [HttpPost]
        [Authorize]
        [Route("clients/add")]
        public APIResult<string> Post(Client oClient)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    ClientDAO.Insert(oClient);

                    return new APIResult<string>()
                    {
                        Message = "Cliente inserido com sucesso!"
                    };
                }
                else
                    return new APIResult<string>()
                    {
                        Success = false,
                        Message = "Acesso negado!"
                    };
            }
            catch (Exception e)
            {
                return new APIResult<string>()
                {
                    Success = false,
                    Message = "Erro ao inserir novo cliente! " + e.Message,
                };
            }
        }
    }
}