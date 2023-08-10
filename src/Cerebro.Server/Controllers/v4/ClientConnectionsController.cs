using Cerebro.Core.Abstractions.Services;
using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Dtos.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;

namespace Cerebro.Server.Controllers.v4
{
    [Route("api/v4/client-connections")]
    [ApiController]
    public class ClientConnectionsController : ControllerBase
    {
        private readonly ILogger<ClientConnectionsController> _logger;
        private readonly IClientConnectionService _clientConnectionService;

        public ClientConnectionsController(ILogger<ClientConnectionsController> logger, IClientConnectionService clientConnectionService)
        {
            _logger = logger;
            _clientConnectionService = clientConnectionService;
        }


        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<ClientConnectionDto>> GetClientConnections([FromQuery] string? applicationName, [FromQuery] string? addressAlias, [FromQuery] string? addressName)
        {
            if (applicationName != null)
            {
                (var clients, string message) = _clientConnectionService.GetClientConnectionsByApplicationName(applicationName!);
                if (clients == null)
                    return NotFound(message);

                return Ok(clients);
            }

            if (addressAlias != null)
            {
                (var clients, string message) = _clientConnectionService.GetClientConnectionsByAddressAlias(addressAlias);
                if (clients == null)
                    return NotFound(message);

                return Ok(clients);
            }

            if (addressName != null)
            {
                string addressNameUnescape = Uri.UnescapeDataString(addressName);


                (var clients, string message) = _clientConnectionService.GetClientConnectionsByAddressName(addressNameUnescape);
                if (clients == null)
                    return NotFound(message);

                return Ok(clients);
            }

            return BadRequest("All query parameters cannot be null");
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> PostClientConnection([FromBody] ClientConnectionRequest clientConnectionRequest)
        {
            (bool status, string message) = _clientConnectionService.RegisterClientConnection(clientConnectionRequest, "system");
            if (status != true)
                return BadRequest(message);

            return Ok(message);
        }

        [HttpGet("verify")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> VerifyClientConnection([FromQuery] string applicationName, [FromQuery] string? addressAlias, [FromQuery] string? addressName, [FromQuery] ApplicationConnectionTypes applicationConnectionType)
        {
            if (addressAlias != null)
            {
                (var status, string message) = _clientConnectionService.VerifyClientConnectionByAddressAlias(applicationName, addressAlias, applicationConnectionType);
                if (status != true)
                    return NotFound(message);

                return Ok(message);
            }

            if (addressName != null)
            {
                string addressNameUnescape = Uri.UnescapeDataString(addressName);


                (var status, string message) = _clientConnectionService.VerifyClientConnectionByAddressName(applicationName, addressNameUnescape, applicationConnectionType);
                if (status != true)
                    return NotFound(message);

                return Ok(message);
            }

            return BadRequest("Something went wrong");
        }
    }
}
