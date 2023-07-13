using Cerebro.Core.Models.Common.System;
using Cerebro.Core.Models.Configurations;
using Cerebro.Core.Utilities.Consts;
using Microsoft.AspNetCore.Mvc;

namespace Cerebro.Api.Controllers.v4
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
    }
}
