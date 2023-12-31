﻿using Vortex.Core.Models.Common.System;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Utilities.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Vortex.Server.Controllers.v4
{
    [Route("api/v4/node")]
    [ApiController]
    public class NodeController : ControllerBase
    {
        private readonly ILogger<NodeController> _logger;
        private readonly NodeConfiguration _nodeConfiguration;

        public NodeController(ILogger<NodeController> logger, NodeConfiguration nodeConfiguration)
        {
            _logger = logger;
            _nodeConfiguration = nodeConfiguration;
        }

        [HttpGet("version")]
        public ActionResult<SystemDetails> GetSystemDetails()
        {
            return Ok(new SystemDetails()
            {
                Name = SystemProperties.Name,
                ShortName = SystemProperties.ShortName,
                Version = SystemProperties.Version
            });
        }

        [HttpGet("configuration")]
        public ActionResult<NodeConfiguration> GetConfigurationDetails()
        {
            return Ok(_nodeConfiguration);
        }

        [HttpGet("health")]
        public ActionResult<string> GetHealthDetails()
        {
            return Ok("Healthy");
        }

        [HttpGet("status")]
        [Authorize(Roles = "Admin,Readonly")]
        public ActionResult<string> GetNodeStatus()
        {
            return Ok("NOT_IMPLEMENTED; statuses will be [Online, Starting, Offline, Recovering]");
        }
    }
}
