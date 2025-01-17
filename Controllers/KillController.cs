using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mole.API.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Mole.API.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class KillController : Controller
    {
        private readonly ComputationManager manager;


        public KillController(IOptions<ComputationManager> optionsAccessor)
        {
            manager = optionsAccessor.Value;
        }

        [HttpGet("{computationId}")]
        public string Get(string computationId)
        {
            var computation = manager.LoadComputation(computationId);
            if (computation != null)
            {
                var report = computation.GetComputationReport();
                return manager.KillComputation(computation).ToJson();
            }

            return Computation.NotExists(computationId).ToJson();
        }
    }
}