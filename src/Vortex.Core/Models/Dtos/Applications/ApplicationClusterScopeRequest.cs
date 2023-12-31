﻿using Vortex.Core.Abstractions.Background;
using Vortex.Core.Models.Entities.Clients.Applications;

namespace Vortex.Core.Models.Dtos.Applications
{
    public class ApplicationClusterScopeRequest : RequestBase
    {
        public ApplicationDto ApplicationDto { get; set; }

        public ApplicationToken? ApplicationToken { get; set; }
        public bool? ApplicationIsActive { get; set; }

        public ApplicationClusterScopeRequestState State { get; set; }

        public string ApplicationPermissionKey { get; set; }
        public string ApplicationPermissionValue { get; set; }

        public string RequestedBy { get; set; }

        public ApplicationClusterScopeRequest()
        {
            ApplicationToken = null;
            ApplicationIsActive = null;
        }
    }

    public enum ApplicationClusterScopeRequestState
    {
        ApplicationCreationRequested,
        ApplicationSoftDeletionRequested,
        ApplicationHardDeletionRequested,
        ApplicationDescriptionChangeRequest,
        ApplicationSettingsChangeRequest,

        ApplicationActivationRequest,
        ApplicationTokenCreationRequest,
        ApplicationTokenRevocationRequest,
        ApplicationPermissionChangeRequest
    }
}
