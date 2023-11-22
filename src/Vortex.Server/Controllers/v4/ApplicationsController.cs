using Vortex.Core.Abstractions.Services;
using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Dtos.Applications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Vortex.Server.Controllers.v4
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
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PostApplication([FromBody] ApplicationDto applicationDto,
            [FromQuery] bool IsWithPermissions,
            [FromQuery] string? readAddressPermission,
            [FromQuery] string? writeAddressPermission,
            [FromQuery] bool? createAddressPermission)
        {
            if (applicationDto == null)
                return BadRequest("Body request cannot be null");

            (bool isCreated, string message) = _applicationService.CreateApplication(applicationDto, "system");
            if (isCreated)
            {
                if (IsWithPermissions == true)
                    _applicationService.CreateApplicationPermission(applicationDto.Name, readAddressPermission, writeAddressPermission, createAddressPermission, "system");

                return Ok(message);
            }

            return BadRequest(message);
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin,Readonly")]
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
        [Authorize(Roles = "Admin,Readonly")]
        public ActionResult<List<ApplicationDto>> GetApplications(string applicationName)
        {
            (var application, string message) = _applicationService.GetApplication(applicationName);
            if (application == null)
                return NotFound(message);

            return Ok(application);
        }

        [HttpPost("{applicationName}/promote")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PromoteApplication(string applicationName)
        {
            (bool isPromoted, string message) = _applicationService.PromoteApplication(applicationName, "system");
            if (isPromoted)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpPut("{applicationName}/settings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PutApplicationSettings(string applicationName, [FromBody] ApplicationSettings applicationSettings)
        {
            (var result, string message) = _applicationService.EditApplicationSettings(applicationName, applicationSettings, "system");
            if (result == true)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpPut("{applicationName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PutApplicationSettings(string applicationName, [FromQuery] string applicationDescription)
        {
            (var result, string message) = _applicationService.EditApplicationDescription(applicationName, applicationDescription, "system");
            if (result == true)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpPut("{applicationName}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PutDeactivateApplication(string applicationName)
        {
            (var result, string message) = _applicationService.DeactivateApplication(applicationName, "system");
            if (result == true)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpPut("{applicationName}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PutActivateApplication(string applicationName)
        {
            (var result, string message) = _applicationService.ActivateApplication(applicationName, "system");
            if (result == true)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpDelete("applicationName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> DeleteApplication(string applicationName, [FromQuery] bool isSoftDelete)
        {
            bool result;
            string message;
            if (isSoftDelete)
                (result, message) = _applicationService.SoftDeleteApplication(applicationName, "system");
            else
                (result, message) = _applicationService.HardDeleteApplication(applicationName);

            if (result == true)
                return Ok(message);

            return BadRequest(message);
        }



        [HttpPost("{applicationName}/tokens")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<TokenResponse> PostApplicationToken(string applicationName, [FromBody] TokenRequest request)
        {
            (var result, string message) = _applicationService.CreateApplicationToken(applicationName, request, "system");

            if (result == null)
                return BadRequest(message);

            return Ok(result);
        }

        [HttpGet("{applicationName}/tokens")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Readonly")]
        public ActionResult<List<ApplicationTokenDto>> GetApplicationTokens(string applicationName)
        {
            (var result, string message) = _applicationService.GetApplicationTokens(applicationName);
            if (result == null)
                return BadRequest(message);

            return Ok(result);
        }

        [HttpGet("{applicationName}/tokens/{apiKey}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Readonly")]
        public ActionResult<ApplicationTokenDto> GetApplicationToken(string applicationName, Guid apiKey)
        {
            (var result, string message) = _applicationService.GetApplicationToken(applicationName, apiKey);
            if (result == null)
                return BadRequest(message);

            return Ok(result);
        }

        [HttpPut("{applicationName}/tokens/{apiKey}/revoke")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> RevokeApplicationToken(string applicationName, Guid apiKey)
        {
            (var status, string message) = _applicationService.RevokeApplicationToken(applicationName, apiKey, "system");
            if (status == true)
                return Ok(message);

            return BadRequest(message);
        }


        [HttpGet("{applicationName}/permissions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Readonly")]
        public ActionResult<ApplicationPermissionDto> GetApplicationPermissions(string applicationName)
        {
            (var result, string message) = _applicationService.GetApplicationPermissions(applicationName);
            if (result != null)
                return Ok(result);

            return BadRequest(message);
        }

        [HttpPut("{applicationName}/permissions/read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PutReadAddressPermission(string applicationName, [FromBody] string value)
        {
            (var status, string message) = _applicationService.EditReadAddressApplicationPermission(applicationName, value, "system");
            if (status == true)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpPut("{applicationName}/permissions/write")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PutWriteAddressPermission(string applicationName, [FromBody] string value)
        {
            (var status, string message) = _applicationService.EditWriteAddressApplicationPermission(applicationName, value, "system");
            if (status == true)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpPut("{applicationName}/permissions/create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PutCreateAddressPermission(string applicationName, [FromBody] bool value)
        {
            (var status, string message) = _applicationService.EditCreateAddressApplicationPermission(applicationName, value, "system");
            if (status == true)
                return Ok(message);

            return BadRequest(message);
        }
    }
}
