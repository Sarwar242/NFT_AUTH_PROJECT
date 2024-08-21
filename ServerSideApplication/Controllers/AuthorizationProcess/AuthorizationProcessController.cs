using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelClasses.NftAuth;
using ServerSideApplication.Service.AuthorizationProcess;

namespace ServerSideApplication.Controllers.AuthorizationProcess
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizationProcessController : ControllerBase
    {
        private readonly IAuthorizationProcess _authorizationProcess;

        public AuthorizationProcessController(IAuthorizationProcess authorizationProcess)
        {
            _authorizationProcess = authorizationProcess;
        }

        [HttpGet]
        [Route("GetAllEmployeeList")]
        public async Task<IActionResult> GetAllUserList() {

            return Ok(await _authorizationProcess.GetAllUserList());
        }

        [HttpGet]
        [Route("GetLogTable")]
        public async Task<IActionResult> GetTableLog(string name,string branch_id)
        {
            List<AuthLogModel> _list = new List<AuthLogModel>();

            _list = await _authorizationProcess.GetAuthLogTableName(name,branch_id);
            return Ok(_list);
        }


        [HttpGet]
        [Route("GetTableLogData")]
        public async Task<IActionResult> GetTableLogData(string name, string branch_id,string function_id)
        {
            List<AuthLogModel> _list = new List<AuthLogModel>();

            _list = await _authorizationProcess.GetAuthLogTableDataList(name, branch_id, function_id);
            return Ok(_list);
        }
    }
}
