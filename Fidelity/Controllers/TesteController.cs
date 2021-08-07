using Fidelity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Fidelity.Controllers
{
    public class TesteController : ApiController
    {
        //public async Task<JsonResult> Get()
        //{
        //    try PEPPA PIG POLICIAL
        //    {
        //        dynamic json = new string[] { "Teste", "Teste 2" };
        //        return new JsonResult() { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Erro: " + e.Message);
        //    }
        //}

        //private static List<Client> oClients = new List<Client>();
        //private static int count = 0;

        //[System.Web.Http.HttpGet]
        //[System.Web.Http.Route("clients")]
        //public APIResult<List<Client>> Get()
        //{
        //    try
        //    {
        //        var oResult = new APIResult<List<Client>>
        //        {
        //            Count = oClients.Count,
        //            Object = oClients
        //        };

        //        return oResult;
        //    }
        //    catch (Exception e)
        //    {
        //        return new APIResult<List<Client>>()
        //        {
        //            Success = false,
        //            Message = "Erro ao buscar todos clientes: " + e.Message,
        //        };
        //    }
        //}

        //[System.Web.Http.HttpPost]
        //[System.Web.Http.Route("clients/add")]
        //public APIResult<string> Post(Client oClient)
        //{
        //    try
        //    {
        //        using (var contexto = new ApplicationDbContext())
        //        {
        //            contexto.DbSetClient.Add(oClient);
        //            contexto.SaveChanges();
        //        }

        //        return new APIResult<string>()
        //        {
        //            Message = "Cliente inserido com sucesso!"
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        return new APIResult<string>()
        //        {
        //            Success = false,
        //            Message = "Erro ao inserir novo cliente! " + e.Message,
        //        };
        //    }
        //}
    }
}