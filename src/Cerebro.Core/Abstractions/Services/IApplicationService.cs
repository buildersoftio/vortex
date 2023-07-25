using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Dtos.Applications;

namespace Cerebro.Core.Abstractions.Services
{
    public interface IApplicationService
    {
        (bool status, string message) CreateApplication(ApplicationDto newApplication, string createdBy);
        (bool status, string message) EditApplicationSettings(string applicationName, ApplicationSettings newApplicationSettings, string updatedBy);
        (bool status, string message) EditApplicationDescription(string applicationName, string newDescription, string updatedBy);

        (bool status, string message) ActivateApplication(string applicationName, string createdBy);
        (bool status, string message) DeactivateApplication(string applicationName, string createdBy);

        (bool status, string message) SoftDeleteApplication(string applicationName, string createdBy);
        (bool status, string message) HardDeleteApplication(string applicationName);

        (ApplicationDto? application, string message) GetApplication(string applicationName);
        (List<ApplicationDto> applicationDtos, string message) GetApplications();
        (List<ApplicationDto> applicationDtos, string message) GetActiveApplications();



        (TokenResponse? token, string message) CreateApplicationToken(string applicationName, TokenRequest tokenRequest, string createdBy);
        (ApplicationTokenDto? applicationToken, string message) GetApplicationToken(string applicationName, Guid appKey);
        (List<ApplicationTokenDto>? applicationTokens, string message) GetApplicationTokens(string applicationName);
        (bool status, string message) RevokeApplicationToken(string applicationName, Guid appKey, string updateBy);



        (ApplicationPermissionDto? permissionDto, string message) GetApplicationPermissions(string applicationName);
        (bool status, string message) EditReadAddressApplicationPermission(string applicationName, string permission, string updatedBy);
        (bool status, string message) EditWriteAddressApplicationPermission(string applicationName, string permission, string updatedBy);
        (bool status, string message) EditCreateAddressApplicationPermission(string applicationName, bool permission, string updatedBy);
    }
}
