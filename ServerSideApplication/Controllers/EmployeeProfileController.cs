using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelClasses;
using ServerSideApplication.Service.EmployeeProfile;
using System.Runtime.InteropServices;

namespace ServerSideApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeProfileController : ControllerBase
    {
        private readonly IEmployeeProfileService _EmpService;
        public EmployeeProfileController(IEmployeeProfileService EmpService)
        {
            _EmpService = EmpService;  
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployee()
        {
            var EmployeeList=await _EmpService.GetAllEmployeesAsync();
            return Ok(EmployeeList);
        }

        [HttpPost]
        public async Task<IActionResult> CreatEmployeeProfile(Employee employee)
        {
            var EmployeeList= await _EmpService.CreateEmployee(employee);
            return Ok(EmployeeList);
        }

    }
}
