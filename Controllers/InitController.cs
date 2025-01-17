﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mole.API.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;

namespace Mole.API.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class InitController : Controller
    {
        private readonly ComputationManager manager;

        public InitController(IOptions<ComputationManager> optionsAccessor)
        {
            manager = optionsAccessor.Value;
        }


        [HttpGet("{id}")]
        public string Get(string id, string assemblyId)
        {
            var result = manager.CreatePDBComputation(id.ToLower(), assemblyId);
            return result.ToJson();
        }


        [HttpGet("Pores/{id}", Name = "Init/Pores")]
        public string Get(string id)
        {
            var result = manager.CreatePoreComputation(id.ToLower());            
            return result.ToJson();
        }



        [HttpPost]
        [Consumes("multipart/form-data")]
        public string Post(IFormFile file)
        {
            if (!Regex.Match(file.FileName.ToLower(), "(\\.pdb|\\.cif|\\.gz|\\.pdb[0-9]+)$").Success)
            {
                return new ComputationReport()
                {
                    ComputationId = null,
                    ErrorMsg = "Unsupported file type. Supported extensions are *.pdb, *.cif, *.pdbX (e.g. *.pdb1) and *.gz",
                    Status = ComputationStatus.Error,
                    SubmitId = 0
                }.ToJson();
            }
            var result = manager.CreateUserComputation(file);

            return result.ToJson();
        }






    }
}