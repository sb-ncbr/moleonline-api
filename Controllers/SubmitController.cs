using System;
using Mole.API;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mole.API.Models;
using Newtonsoft.Json;

namespace Mole.API.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class SubmitController : Controller
    {
        private readonly ComputationManager manager;


        public SubmitController(IOptions<ComputationManager> optionsAccessor)
        {
            manager = optionsAccessor.Value;
        }




        [HttpPost("Mole/{computationId}", Name = "Submit/Mole")]
        public string Post(string computationId, [FromBody] ComputationParameters param)
        {
            Task.Run(() => LogIp($"Mole|{computationId}"));

            var cpt = manager.LoadComputation(computationId);
            if (cpt == null) return Computation.NotExists(computationId).ToJson();
            if (param == null) return new ComputationReport()
            {
                ComputationId = cpt.ComputationId,
                Status = ComputationStatus.Error,
                ErrorMsg = "Input format error. Request's body could not be serialized, please fix the error and try again."
            }.ToJson();


            var canRunMsg = manager.CanRun(cpt);

            if (!String.IsNullOrEmpty(canRunMsg))
            {
                return
                    new ComputationReport()
                    {
                        ComputationId = computationId,
                        SubmitId = 0,
                        Status = ComputationStatus.Error,
                        ErrorMsg = canRunMsg
                    }.ToJson();
            }

            cpt.AddCalculation();

            if (param == null) param = new ComputationParameters();

            var report = manager.PrepareAndRunMole(cpt, param);

            return report.ToJson();
        }



        [HttpPost("Pores/{computationId}", Name = "Submit/Pores")]
        public string Pores(string computationId, [FromBody] APIPoresParameters parameters)
        {
            Task.Run(() => LogIp($"Pores|{computationId}"));

            var cpt = manager.LoadComputation(computationId);
            if (cpt == null) return Computation.NotExists(computationId).ToJson();

            var canRunMsg = manager.CanRun(cpt);

            if (!String.IsNullOrEmpty(canRunMsg))
            {
                return
                    new ComputationReport()
                    {
                        ComputationId = computationId,
                        SubmitId = 0,
                        Status = ComputationStatus.Error,
                        ErrorMsg = canRunMsg
                    }.ToJson();
            }

            cpt.AddCalculation();

            var result = manager.PrepareAndRunPores(cpt, parameters);

            return result.ToJson();

        }


        private void LogIp(string s)
        {
            System.IO.File.AppendAllText("Mole_api_IP_log.csv", $"{DateTime.Now} Submit - {Request.HttpContext.Connection.RemoteIpAddress.ToString()}\n");
        }
    }
}