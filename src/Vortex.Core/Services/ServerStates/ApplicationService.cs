using Vortex.Core.Abstractions.Background;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Models.Common.Clients.Applications;
using Vortex.Core.Models.Dtos.Applications;
using Vortex.Core.Models.Entities.Clients.Applications;
using Vortex.Core.Repositories;
using Vortex.Core.Utilities.Consts;
using Vortex.Core.Utilities.Extensions;
using Vortex.Core.Utilities.Validators;
using Microsoft.Extensions.Logging;

namespace Vortex.Core.Services.ServerStates
{
    public class ApplicationService : IApplicationService
    {
        private readonly ILogger<ApplicationService> _logger;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IBackgroundQueueService<ApplicationClusterScopeRequest> _backgroundApplicationClusterService;

        public ApplicationService(ILogger<ApplicationService> logger,
            IApplicationRepository applicationRepository,
            IBackgroundQueueService<ApplicationClusterScopeRequest> backgroundApplicationClusterService)
        {
            _logger = logger;
            _applicationRepository = applicationRepository;

            _backgroundApplicationClusterService = backgroundApplicationClusterService;
        }

        public (bool status, string message) CreateApplication(ApplicationDto newApplication, string createdBy, bool requestedByOtherNode = false)
        {
            var application = _applicationRepository.GetApplication(newApplication.Name.ToReplaceDuplicateSymbols());
            if (application != null)
                return (status: false, message: $"Application {newApplication.Name} already exists");

            if (newApplication!.Settings.PrivateIpRange.IsValidIpAddress() != true)
                return (status: false, message: $"Application {newApplication.Name} cannot register, PrivateIpRange is not a list of ip addresses");

            if (newApplication!.Settings.PublicIpRange.IsValidIpAddress() != true)
                return (status: false, message: $"Application {newApplication.Name} cannot register, PublicIpRange is not a list of ip addresses");

            // group ips in case there can be some duplicate

            application = new Application()
            {
                // remove duplicate symbols
                Name = newApplication.Name.ToReplaceDuplicateSymbols(),
                Description = newApplication.Description,
                Settings = newApplication.Settings,
                CreatedBy = createdBy,
            };

            if (_applicationRepository.AddApplication(application))
            {
                // create default permissions for this application
                _applicationRepository.AddApplicationPermission(DefaultApplicationPermissions.CreateDefaultApplicationPermissionEntity(application.Id, createdBy));

                //in case application is created and ApplicationScope is ClusterScope here we will inform other ndoes.
                if (application.Settings.Scope == ApplicationScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundApplicationClusterService.EnqueueRequest(new ApplicationClusterScopeRequest()
                    {
                        ApplicationDto = newApplication,
                        RequestedBy = createdBy,
                        State = ApplicationClusterScopeRequestState.ApplicationCreationRequested
                    });
                }

                return (status: true, message: $"Application {application.Name} has been created successfully with id {application.Id}");
            }

            return (status: false, message: $"Application has not been created.");
        }

        public (bool status, string message) CreateApplicationPermission(string applicationName, string? read, string? write, bool? create, string createdBy)
        {
            if (read != null)
                EditReadAddressApplicationPermission(applicationName, read, createdBy);
            if (write != null)
                EditWriteAddressApplicationPermission(applicationName, write, createdBy);
            if (create != null)
                EditCreateAddressApplicationPermission(applicationName, create.Value, createdBy);

            return (true, "Permission updated");
        }


        public (bool status, string message) EditApplicationDescription(string applicationName, string newDescription, string updatedBy, bool requestedByOtherNode = false)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return (status: false, message: $"Application {applicationName} doesnot exists");

            if (application.IsDeleted == true)
                return (status: false, message: $"Application {applicationName} has been softly deleted, description cannot be changed");

            application.Description = newDescription;
            application.UpdatedAt = DateTimeOffset.UtcNow;
            application.UpdatedBy = updatedBy;

            if (_applicationRepository.UpdateApplication(application))
            {
                // Sync with other nodes
                if (application.Settings.Scope == ApplicationScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundApplicationClusterService.EnqueueRequest(new ApplicationClusterScopeRequest()
                    {
                        ApplicationDto = new ApplicationDto() { Description = newDescription, Name = applicationName, Settings = application.Settings },
                        RequestedBy = updatedBy,
                        State = ApplicationClusterScopeRequestState.ApplicationDescriptionChangeRequest
                    });
                }

                return (status: true, message: $"Application {applicationName} description is updated");
            }

            return (status: false, message: $"Application {applicationName} description couldnot update");
        }

        public (bool status, string message) EditApplicationSettings(string applicationName, ApplicationSettings newApplicationSettings, string updatedBy, bool requestedByOtherNode = false)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return (status: false, message: $"Application {applicationName} doesnot exists");

            if (application.IsDeleted == true)
                return (status: false, message: $"Application {applicationName} has been softly deleted, settings of a deleted application cannot be changed");

            if (newApplicationSettings.PrivateIpRange.IsValidIpAddress() != true)
                return (status: false, message: $"Application {applicationName} cannot change, PrivateIpRange is not a list of ip addresses");

            if (newApplicationSettings.PublicIpRange.IsValidIpAddress() != true)
                return (status: false, message: $"Application {applicationName} cannot change, PublicIpRange is not a list of ip addresses");

            if (newApplicationSettings.Scope != application.Settings.Scope)
                return (status: false, message: $"Application {applicationName} cannot change, Scope can not change without promoting");


            application.Settings = newApplicationSettings;
            application.UpdatedAt = DateTimeOffset.UtcNow;
            application.UpdatedBy = updatedBy;

            if (_applicationRepository.UpdateApplication(application))
            {
                // Sync with other nodes
                if (application.Settings.Scope == ApplicationScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundApplicationClusterService.EnqueueRequest(new ApplicationClusterScopeRequest()
                    {
                        ApplicationDto = new ApplicationDto() { Description = application.Description, Name = applicationName, Settings = application.Settings },
                        RequestedBy = updatedBy,
                        State = ApplicationClusterScopeRequestState.ApplicationSettingsChangeRequest
                    });
                }

                return (status: true, message: $"Application {applicationName} settings is updated");
            }

            return (status: false, message: $"Application {applicationName} settings couldnot update");
        }

        public (bool status, string message) PromoteApplication(string applicationName, string updatedBy, bool requestedByOtherNode = false)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return (status: false, message: $"Application {applicationName} doesnot exists");

            if (application.Settings.Scope == ApplicationScope.ClusterScope)
                return (status: false, message: $"Application {applicationName} is already promoted to ClusterScope");

            application.Settings.Scope = ApplicationScope.ClusterScope;
            application.UpdatedAt = DateTimeOffset.UtcNow;
            application.UpdatedBy = updatedBy;

            if (_applicationRepository.UpdateApplication(application))
            {
                // Sync with other nodes
                if (application.Settings.Scope == ApplicationScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundApplicationClusterService.EnqueueRequest(new ApplicationClusterScopeRequest()
                    {
                        ApplicationDto = new ApplicationDto() { Description = application.Description, Name = applicationName, Settings = application.Settings },
                        RequestedBy = updatedBy,
                        State = ApplicationClusterScopeRequestState.ApplicationCreationRequested
                    });
                }

                return (status: false, message: $"Application {applicationName} promoted to ClusterScope");
            }

            return (status: false, message: $"Application {applicationName} couldnot promoted");
        }

        public (ApplicationDto? application, string message) GetApplication(string applicationName)
        {
            var applicationDetails = _applicationRepository.GetApplication(applicationName);
            if (applicationDetails == null)
                return (application: null, message: $"Application {applicationName} doesnot exists");

            if (applicationDetails.IsDeleted == true)
                return (application: null, message: $"Application {applicationName} doesnot exists, is softly deleted");

            return (application: new ApplicationDto(applicationDetails), message: $"Application returned");
        }

        public (List<ApplicationDto> applicationDtos, string message) GetApplications()
        {
            var applications = _applicationRepository
                .GetApplications()
                .Select(a => new ApplicationDto(a))
                .ToList();
            return (applicationDtos: applications, message: "Applications returned");
        }

        public (List<ApplicationDto> applicationDtos, string message) GetActiveApplications()
        {
            var applications = _applicationRepository
                .GetActiveApplications()
                .Select(a => new ApplicationDto(a))
                .ToList();

            return (applicationDtos: applications, message: "Active and non deleted applications returned");
        }

        public (bool status, string message) HardDeleteApplication(string applicationName, bool requestedByOtherNode = false)
        {
            var applicationDetails = _applicationRepository.GetApplication(applicationName);
            if (applicationDetails == null)
                return (false, message: $"Application {applicationName} doesnot exists");

            var isDeleted = _applicationRepository.DeleteApplication(applicationDetails);

            // revoke applicationTokens
            var tokens = _applicationRepository.GetApplicationTokens(applicationDetails.Id);
            tokens.ForEach(token =>
            {
                _applicationRepository.DeleteApplicationToken(token);
            });

            if (isDeleted)
            {
                // Sync with other nodes
                if (applicationDetails.Settings.Scope == ApplicationScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundApplicationClusterService.EnqueueRequest(new ApplicationClusterScopeRequest()
                    {
                        ApplicationDto = new ApplicationDto() { Description = applicationDetails.Description, Name = applicationName, Settings = applicationDetails.Settings },
                        RequestedBy = "na",
                        State = ApplicationClusterScopeRequestState.ApplicationHardDeletionRequested
                    });
                }

                return (true, message: $"Application deleted together with tokens");
            }

            return (false, message: $"Something went wrong, application couldnot be deleted");

        }

        public (bool status, string message) SoftDeleteApplication(string applicationName, string updatedBy, bool requestedByOtherNode = false)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return (false, message: $"Application {applicationName} doesnot exists");


            application.IsActive = false;
            application.IsDeleted = true;
            application.UpdatedAt = DateTimeOffset.UtcNow;
            application.UpdatedBy = updatedBy;

            var isSoftDeleted = _applicationRepository.UpdateApplication(application);

            // revoke applicationTokens
            var tokens = _applicationRepository.GetApplicationTokens(application.Id);
            tokens.ForEach(token =>
            {
                token.IsActive = false;
                token.IsDeleted = true;
                token.UpdatedAt = DateTimeOffset.UtcNow;
                token.UpdatedBy = updatedBy;
                _applicationRepository.UpdateApplicationToken(token);
            });

            if (isSoftDeleted)
            {
                // Sync with other nodes
                if (application.Settings.Scope == ApplicationScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundApplicationClusterService.EnqueueRequest(new ApplicationClusterScopeRequest()
                    {
                        ApplicationDto = new ApplicationDto() { Description = application.Description, Name = applicationName, Settings = application.Settings },
                        RequestedBy = updatedBy,
                        State = ApplicationClusterScopeRequestState.ApplicationSoftDeletionRequested
                    });
                }

                return (true, message: $"Application {applicationName} is softly deleted, tokens related to this application are revoked");
            }

            return (false, message: $"Something went wrong, application couldnot be softly deleted");
        }

        public (bool status, string message) ActivateApplication(string applicationName, string createdBy, bool requestedByOtherNode = false)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return (false, message: $"Application {applicationName} doesnot exists");

            if (application.IsDeleted == true)
                return (status: false, message: $"Application {applicationName} has been softly deleted, activation cannot happen");

            application.IsActive = true;
            application.UpdatedAt = DateTimeOffset.UtcNow;
            application.UpdatedBy = createdBy;

            if (_applicationRepository.UpdateApplication(application))
            {
                // Sync with other nodes
                if (application.Settings.Scope == ApplicationScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundApplicationClusterService.EnqueueRequest(new ApplicationClusterScopeRequest()
                    {
                        ApplicationDto = new ApplicationDto() { Description = application.Description, Name = applicationName, Settings = application.Settings },
                        ApplicationIsActive = true,
                        RequestedBy = createdBy,
                        State = ApplicationClusterScopeRequestState.ApplicationActivationRequest
                    });
                }

                return (status: true, message: $"Application {applicationName} is activated");
            }

            return (status: false, message: $"Application {applicationName} could not be activated");

        }

        public (bool status, string message) DeactivateApplication(string applicationName, string createdBy, bool requestedByOtherNode = false)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return (false, message: $"Application {applicationName} doesnot exists");

            if (application.IsDeleted == true)
                return (status: false, message: $"Application {applicationName} has been softly deleted, deactivation cannot happen");

            application.IsActive = false;
            application.UpdatedAt = DateTimeOffset.UtcNow;
            application.UpdatedBy = createdBy;

            if (_applicationRepository.UpdateApplication(application))
            {

                // Sync with other nodes
                if (application.Settings.Scope == ApplicationScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundApplicationClusterService.EnqueueRequest(new ApplicationClusterScopeRequest()
                    {
                        ApplicationDto = new ApplicationDto() { Description = application.Description, Name = applicationName, Settings = application.Settings },
                        ApplicationIsActive = false,
                        RequestedBy = createdBy,
                        State = ApplicationClusterScopeRequestState.ApplicationActivationRequest
                    });
                }

                return (status: true, message: $"Application {applicationName} is deactivated");
            }

            return (status: false, message: $"Application {applicationName} could not be deactivated");
        }

        public (TokenResponse? token, string message) CreateApplicationToken(string applicationName, TokenRequest tokenRequest, string createdBy, bool requestedByOtherNode = false)
        {
            (var application, string message) = GetApplication(applicationName);
            if (application == null)
                return (null, message: message);


            // generate new key
            string secret = CryptographyExtensions.GenerateApiSecret();
            string hashedSecret;
            switch (tokenRequest.CryptographyType)
            {
                case Models.Common.CryptographyTypes.SHA256:
                    hashedSecret = secret.ToSHA256_HashString();
                    break;
                case Models.Common.CryptographyTypes.SHA384:
                    hashedSecret = secret.ToSHA384_HashString();
                    break;
                case Models.Common.CryptographyTypes.SHA512:
                    hashedSecret = secret.ToSHA512_HashString();
                    break;
                default:
                    hashedSecret = secret.ToSHA512_HashString();
                    break;
            }

            var applicationToken = new ApplicationToken()
            {
                Id = Guid.NewGuid(),
                ApplicationId = application.Id,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = createdBy,
                CryptographyType = tokenRequest.CryptographyType,
                HashedSecret = hashedSecret,
                Description = tokenRequest.Description,
                ExpireDate = tokenRequest.ExpireDate,
                IssuedDate = DateTimeOffset.UtcNow
            };

            if (_applicationRepository.AddApplicationToken(applicationToken))
            {
                // Sync with other nodes
                if (application.Settings.Scope == ApplicationScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundApplicationClusterService.EnqueueRequest(new ApplicationClusterScopeRequest()
                    {
                        ApplicationDto = new ApplicationDto() { Description = application.Description, Name = applicationName, Settings = application.Settings },
                        ApplicationToken = applicationToken,
                        RequestedBy = createdBy,
                        State = ApplicationClusterScopeRequestState.ApplicationTokenCreationRequest
                    });
                }

                return (new TokenResponse()
                {
                    ApplicationName = applicationName,
                    ExpireDate = applicationToken.ExpireDate,
                    Key = applicationToken.Id,
                    Secret = secret
                }, "Application Token Created");
            }


            return (token: null, message: $"Something went wrong, application token couldnot be created");
        }

        public (bool status, string message) CreateInternalApplicationToken(string applicationName, ApplicationToken applicationToken)
        {
            (var application, string message) = GetApplication(applicationName);
            if (application == null)
                return (status: false, message: message);

            applicationToken.ApplicationId = application.Id;

            if (_applicationRepository.AddApplicationToken(applicationToken))
                return (status: true, "Application Token Created");

            return (status: true, message: $"Something went wrong, application token couldnot be created");
        }

        public (ApplicationTokenDto? applicationToken, string message) GetApplicationToken(string applicationName, Guid appKey)
        {
            (var application, string message) = GetApplication(applicationName);
            if (application == null)
                return (null, message: message);

            var applicationToken = _applicationRepository.GetApplicationToken(application.Id, appKey);
            if (applicationToken == null)
                return (null, message: $"There is no application token with id {appKey}");


            var applicationTokenDto =
                new ApplicationTokenDto(
                    applicationToken.Id,
                    applicationName,
                    applicationToken.CryptographyType,
                    applicationToken.IsActive,
                    applicationToken.ExpireDate,
                    applicationToken.Description,
                    applicationToken.IssuedDate);

            return (applicationTokenDto, "Application Token returned");
        }

        public (List<ApplicationTokenDto>? applicationTokens, string message) GetApplicationTokens(string applicationName)
        {
            (var application, string message) = GetApplication(applicationName);
            if (application == null)
                return (null, message: message);


            var applicationTokens = _applicationRepository.GetApplicationTokens(application.Id);
            var applicationTokensDto = new List<ApplicationTokenDto>();
            applicationTokens.ForEach(x =>
            {
                applicationTokensDto.Add(new ApplicationTokenDto(x.Id, applicationName, x.CryptographyType, x.IsActive, x.ExpireDate, x.Description, x.IssuedDate));
            });

            return (applicationTokensDto, $"Application Tokens for {applicationName} returned");
        }

        public (bool status, string message) RevokeApplicationToken(string applicationName, Guid appKey, string updateBy, bool requestedByOtherNode = false)
        {
            (var application, string message) = GetApplication(applicationName);
            if (application == null)
                return (false, message: message);

            var applicationToken = _applicationRepository.GetApplicationToken(application.Id, appKey);
            if (applicationToken == null)
                return (false, message: $"There is no application token with id {appKey}");

            applicationToken.IsActive = false;
            applicationToken.UpdatedAt = DateTime.UtcNow;
            applicationToken.UpdatedBy = updateBy;

            if (_applicationRepository.UpdateApplicationToken(applicationToken))
            {
                // Sync with other nodes
                if (application.Settings.Scope == ApplicationScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundApplicationClusterService.EnqueueRequest(new ApplicationClusterScopeRequest()
                    {
                        ApplicationDto = new ApplicationDto() { Description = application.Description, Name = applicationName, Settings = application.Settings },
                        ApplicationToken = applicationToken,
                        RequestedBy = updateBy,
                        State = ApplicationClusterScopeRequestState.ApplicationTokenRevocationRequest
                    });
                }

                return (true, $"Token {appKey} is revoked");
            }


            return (false, message: $"Something went wrong, Application Token is not revoked");
        }

        public (bool status, string message) EditReadAddressApplicationPermission(string applicationName, string permission, string updatedBy, bool requestedByOtherNode = false)
        {
            (var application, string message) = GetApplication(applicationName);
            if (application == null)
                return (false, message: message);

            var applicationPermission = _applicationRepository.GetApplicationPermission(application.Id);
            if (applicationPermission == null)
                return (false, message: "Something went wrong, permission didnot change");

            applicationPermission.Permissions!["READ_ADDRESSES"] = permission;
            applicationPermission.UpdatedAt = DateTime.UtcNow;
            applicationPermission.UpdatedBy = updatedBy;

            if (_applicationRepository.UpdateApplicationPermission(applicationPermission))
            {
                // Sync with other nodes
                if (application.Settings.Scope == ApplicationScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundApplicationClusterService.EnqueueRequest(new ApplicationClusterScopeRequest()
                    {
                        ApplicationDto = new ApplicationDto() { Description = application.Description, Name = applicationName, Settings = application.Settings },
                        ApplicationPermissionKey = "READ_ADDRESSES",
                        ApplicationPermissionValue = permission,
                        RequestedBy = updatedBy,

                        State = ApplicationClusterScopeRequestState.ApplicationPermissionChangeRequest
                    });
                }

                return (true, message: $"READ_ADDRESSES permission has changed for {applicationName}");
            }

            return (false, message: "Something went wrong, READ_ADDRESSES permission didnot change");
        }

        public (bool status, string message) EditWriteAddressApplicationPermission(string applicationName, string permission, string updatedBy, bool requestedByOtherNode = false)
        {
            (var application, string message) = GetApplication(applicationName);
            if (application == null)
                return (false, message: message);

            var applicationPermission = _applicationRepository.GetApplicationPermission(application.Id);
            if (applicationPermission == null)
                return (false, message: "Something went wrong, permission didnot change");

            applicationPermission.Permissions!["WRITE_ADDRESSES"] = permission;
            applicationPermission.UpdatedAt = DateTime.UtcNow;
            applicationPermission.UpdatedBy = updatedBy;

            if (_applicationRepository.UpdateApplicationPermission(applicationPermission))
            {
                // Sync with other nodes
                if (application.Settings.Scope == ApplicationScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundApplicationClusterService.EnqueueRequest(new ApplicationClusterScopeRequest()
                    {
                        ApplicationDto = new ApplicationDto() { Description = application.Description, Name = applicationName, Settings = application.Settings },
                        ApplicationPermissionKey = "WRITE_ADDRESSES",
                        ApplicationPermissionValue = permission,
                        RequestedBy = updatedBy,

                        State = ApplicationClusterScopeRequestState.ApplicationPermissionChangeRequest
                    });
                }

                return (true, message: $"WRITE_ADDRESSES permission has changed for {applicationName}");
            }

            return (false, message: "Something went wrong, WRITE_ADDRESSES permission didnot change");
        }

        public (bool status, string message) EditCreateAddressApplicationPermission(string applicationName, bool permission, string updatedBy, bool requestedByOtherNode = false)
        {
            (var application, string message) = GetApplication(applicationName);
            if (application == null)
                return (false, message: message);

            var applicationPermission = _applicationRepository.GetApplicationPermission(application.Id);
            if (applicationPermission == null)
                return (false, message: "Something went wrong, permission didnot change");

            applicationPermission.Permissions!["CREATE_ADDRESSES"] = permission.ToString();
            applicationPermission.UpdatedAt = DateTime.UtcNow;
            applicationPermission.UpdatedBy = updatedBy;

            if (_applicationRepository.UpdateApplicationPermission(applicationPermission))
            {
                // Sync with other nodes
                if (application.Settings.Scope == ApplicationScope.ClusterScope && requestedByOtherNode != true)
                {
                    _backgroundApplicationClusterService.EnqueueRequest(new ApplicationClusterScopeRequest()
                    {
                        ApplicationDto = new ApplicationDto() { Description = application.Description, Name = applicationName, Settings = application.Settings },
                        ApplicationPermissionKey = "CREATE_ADDRESSES",
                        ApplicationPermissionValue = permission.ToString(),
                        RequestedBy = updatedBy,

                        State = ApplicationClusterScopeRequestState.ApplicationPermissionChangeRequest
                    });
                }

                return (true, message: $"CREATE_ADDRESSES permission has changed for {applicationName}");
            }

            return (false, message: "Something went wrong, CREATE_ADDRESSES permission didnot change");
        }

        public (ApplicationPermissionDto? permissionDto, string message) GetApplicationPermissions(string applicationName)
        {
            (var application, string message) = GetApplication(applicationName);
            if (application == null)
                return (null, message: message);

            var applicationPermission = _applicationRepository.GetApplicationPermission(application.Id);
            if (applicationPermission == null)
                return (null, message: "Something went wrong, permission didnot change");

            return (new ApplicationPermissionDto() { ApplicationName = applicationName, Permissions = applicationPermission.Permissions }, "Permissions returned");
        }

    }
}
