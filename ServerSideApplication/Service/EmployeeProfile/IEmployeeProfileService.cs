using ModelClasses;

namespace ServerSideApplication.Service.EmployeeProfile
{
    public interface IEmployeeProfileService
    {
         Task<List<Employee>> GetAllEmployeesAsync();
        Task<string> CreateEmployee(Employee EmployeeModel);
    }
}
