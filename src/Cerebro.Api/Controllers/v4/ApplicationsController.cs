using Microsoft.AspNetCore.Mvc;

namespace Cerebro.Api.Controllers.v4
{
    [Route("api/v4/clients/applications")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly ILogger<ApplicationsController> _logger;

        public ApplicationsController(ILogger<ApplicationsController> logger)
        {
            _logger = logger;
        }
    }
}
