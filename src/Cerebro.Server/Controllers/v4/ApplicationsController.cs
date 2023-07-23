using Cerebro.Core.Abstractions.Services;
using Cerebro.Core.Models.Dtos.Applications;
using Microsoft.AspNetCore.Mvc;

namespace Cerebro.Server.Controllers.v4
{
    [Route("api/v4/clients/applications")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly ILogger<ApplicationsController> _logger;
        private readonly IApplicationService _applicationService;

        public ApplicationsController(ILogger<ApplicationsController> logger, IApplicationService applicationService)
        {
            _logger = logger;
            _applicationService = applicationService;
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> PostApplication([FromBody] ApplicationDto applicationDto)
        {
            if (applicationDto == null)
                return BadRequest("Body request cannot be null");

            (bool isCreated, string message) = _applicationService.CreateApplication(applicationDto, "system");
            if (isCreated)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ApplicationDto>> GetApplications([FromQuery] bool getAll)
        {
            if (getAll)
            {
                (var applications, string message) = _applicationService.GetApplications();
                return Ok(applications);

            }
            else
            {
                (var applications, string message) = _applicationService.GetActiveApplications();
                return Ok(applications);
            }
        }


        [HttpGet("{applicationName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ApplicationDto>> GetApplications(string applicationName)
        {
            (var application, string message) = _applicationService.GetApplication(applicationName);
            if (application == null)
                return NotFound(message);

            return Ok(application);
        }
    }
}
