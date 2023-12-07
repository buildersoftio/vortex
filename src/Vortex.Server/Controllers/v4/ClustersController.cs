using Microsoft.AspNetCore.Mvc;
using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Models.Common.Clusters;

namespace Vortex.Server.Controllers.v4
{
    [Route("api/v4/cluster")]
    [ApiController]
    public class ClustersController : ControllerBase
    {
        private readonly ILogger<ClustersController> _logger;
        private readonly IClusterStateRepository _clusterStateRepository;

        public ClustersController(ILogger<ClustersController> logger,IClusterStateRepository clusterStateRepository)
        {
            _logger = logger;
            _clusterStateRepository = clusterStateRepository;
        }



        [HttpGet("status")]
        public ActionResult<ClusterStatus> GetClusterStatus()
        {
            return Ok(_clusterStateRepository.GetCluster().Status);
        }

        [HttpGet("")]
        public ActionResult<Core.Models.Common.Clusters.Cluster> GetClusterDetails()
        {
            return Ok(_clusterStateRepository.GetCluster());
        }
    }
}
