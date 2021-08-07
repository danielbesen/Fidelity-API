using Fidelity.Models;
using FidelityLibrary.DataContext;
using FidelityLibrary.Entity;
using FidelityLibrary.Persistance.ClientDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Fidelity.Areas.Clients.Controllers
{
    public class ClientController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("clients")]
        public APIResult<List<Client>> Get()
        {
            try
            {
                var oResult = new APIResult<List<Client>>()
                {
                    Object = ClientDAO.FindAll().ToList(),
                    Count = ClientDAO.FindAll().ToList().Count
                };
                  
                return oResult;
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

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("clients/add")]
        public APIResult<string> Post(Client oClient)
        {
            try
            {
                ClientDAO.Insert(oClient);

                return new APIResult<string>()
                {
                    Message = "Cliente inserido com sucesso!"
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