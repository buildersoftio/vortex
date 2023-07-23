using Cerebro.Core.Abstractions.Services;
using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Dtos.Applications;
using Cerebro.Core.Models.Entities.Clients.Applications;
using Cerebro.Core.Repositories;
using Cerebro.Core.Utilities.Validators;
using Microsoft.Extensions.Logging;

namespace Cerebro.Core.Services.States
{
    public class ApplicationService : IApplicationService
    {
        private readonly ILogger<ApplicationService> _logger;
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationService(ILogger<ApplicationService> logger, IApplicationRepository applicationRepository)
        {
            _logger = logger;
            _applicationRepository = applicationRepository;
        }

        public (bool status, string message) CreateApplication(ApplicationDto newApplication, string createdBy)
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
                return (status: true, message: $"Application {application.Name} has been created successfully with id {application.Id}");

            return (status: false, message: $"Application has not been created.");
        }


        public (bool status, string message) EditApplicationDescription(string applicationName, string newDescription, string updatedBy)
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
                return (status: true, message: $"Application {applicationName} description is updated");

            return (status: false, message: $"Application {applicationName} description couldnot update");
        }

        public (bool status, string message) EditApplicationSettings(string applicationName, ApplicationSettings newApplicationSettings, string updatedBy)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return (status: false, message: $"Application {applicationName} doesnot exists");

            if (application.IsDeleted == true)
                return (status: false, message: $"Application {applicationName} has been softly deleted, settings of a deleted application cannot be changed");

            if (newApplicationSettings.PrivateIpRange.IsValidIpAddress() != true)
                return (status: false, message: $"Application {applicationName} cannot register, PrivateIpRange is not a list of ip addresses");

            if (newApplicationSettings.PublicIpRange.IsValidIpAddress() != true)
                return (status: false, message: $"Application {applicationName} cannot register, PublicIpRange is not a list of ip addresses");


            application.Settings = newApplicationSettings;
            application.UpdatedAt = DateTimeOffset.UtcNow;
            application.UpdatedBy = updatedBy;

            if (_applicationRepository.UpdateApplication(application))
                return (status: true, message: $"Application {applicationName} settings is updated");

            return (status: false, message: $"Application {applicationName} settings couldnot update");
        }

        public (ApplicationDto? application, string message) GetApplication(string applicationName)
        {
            var applicationDetails = _applicationRepository.GetApplication(applicationName);
            if (applicationDetails == null)
                return (application: null, message: $"Application {applicationName} doesnot exists");

            return (application: new ApplicationDto(applicationDetails), message: $"Application returned");
        }

        public (List<ApplicationDto> applicationDtos, string message) GetApplications()
        {
            var applications = _applicationRepository.GetApplications();
            var applicationDtos = new List<ApplicationDto>();
            applications.ForEach(x =>
            {
                applicationDtos.Add(new ApplicationDto(x));
            });
            return (applicationDtos: applicationDtos, message: "Applications returned");
        }

        public (List<ApplicationDto> applicationDtos, string message) GetActiveApplications()
        {
            var applications = _applicationRepository.GetActiveApplications();
            var applicationDtos = new List<ApplicationDto>();
            applications.ForEach(x =>
            {
                applicationDtos.Add(new ApplicationDto(x));
            });
            return (applicationDtos: applicationDtos, message: "Active and non deleted applications returned");
        }


        public (bool status, string message) HardDeleteApplication(string applicationName)
        {
            var applicationDetails = _applicationRepository.GetApplication(applicationName);
            if (applicationDetails == null)
                return (false, message: $"Application {applicationName} doesnot exists");

            var isDeleted = _applicationRepository.DeleteApplication(applicationDetails);
            if (isDeleted)
                return (true, message: $"Application deleted");

            return (false, message: $"Something went wrong, application couldnot be deleted");

        }

        public (bool status, string message) SoftDeleteApplication(string applicationName, string updatedBy)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return (false, message: $"Application {applicationName} doesnot exists");


            application.IsActive = false;
            application.IsDeleted = true;
            application.UpdatedAt = DateTimeOffset.UtcNow;
            application.UpdatedBy = updatedBy;

            var isSoftDeleted = _applicationRepository.UpdateApplication(application);
            if (isSoftDeleted)
                return (true, message: $"Application {applicationName} is softly deleted");
            return (false, message: $"Something went wrong, application couldnot be softly deleted");
        }

        public (bool status, string message) ActivateApplication(string applicationName, string createdBy)
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
                return (status: true, message: $"Application {applicationName} is activated");

            return (status: false, message: $"Application {applicationName} could not be activated");

        }

        public (bool status, string message) DeactivateApplication(string applicationName, string createdBy)
        {
            var application = _applicationRepository.GetApplication(applicationName);
            if (application == null)
                return (false, message: $"Application {applicationName} doesnot exists");

            if (application.IsDeleted == true)
                return (status: false, message: $"Application {applicationName} has been softly deleted, deactivation cannot happen");

            application.IsActive = true;
            application.UpdatedAt = DateTimeOffset.UtcNow;
            application.UpdatedBy = createdBy;

            if (_applicationRepository.UpdateApplication(application))
                return (status: true, message: $"Application {applicationName} is deactivated");

            return (status: false, message: $"Application {applicationName} could not be deactivated");
        }
    }
}
