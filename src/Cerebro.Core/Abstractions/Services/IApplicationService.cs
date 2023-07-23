using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Dtos.Applications;

namespace Cerebro.Core.Abstractions.Services
{
    public interface IApplicationService
    {
        (bool status, string message) CreateApplication(ApplicationDto newApplication, string createdBy);
        (bool status, string message) EditApplicationSettings(string applicationName, ApplicationSettings newApplicationSettings, string updatedBy);
        (bool status, string message) EditApplicationDescription(string applicationName, string newDescription, string updatedBy);
        (bool status, string message) SoftDeleteApplication(string applicationName, string createdBy);
        (bool status, string message) HardDeleteApplication(string applicationName);
        (ApplicationDto? application, string message) GetApplication(string applicationName);
        (List<ApplicationDto>applicationDtos, string message) GetApplications();
    }
}
