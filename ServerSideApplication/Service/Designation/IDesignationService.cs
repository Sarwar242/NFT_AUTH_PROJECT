using ModelClasses;

namespace ServerSideApplication.Service;
public interface IDesignationService
{
    Task<List<DesignationModel>> GetAllAsync();
    Task<string> CreateAsync(DesignationModel designationModel);
}
