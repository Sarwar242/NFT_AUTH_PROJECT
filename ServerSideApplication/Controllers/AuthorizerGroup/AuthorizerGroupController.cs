using Microsoft.AspNetCore.Mvc;
using ModelClasses.AuthorizerGroup;
using ServerSideApplication.Service.AuthorizerGroup;

namespace ServerSideApplication.Controllers.AuthorizerGroup
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizerGroupController : ControllerBase
    {
        private readonly IAuthorizerGroupService _authorizerGroupService;

        public AuthorizerGroupController(IAuthorizerGroupService authorizerGroupService)
        {
            _authorizerGroupService = authorizerGroupService;
        }

        [HttpGet]
        [Route("GetDesignation")]
        public async Task<IActionResult> GetDesignation()
        {
            List<string> designation  = await _authorizerGroupService.GetAllDesignation();
            return Ok(designation);
        }

        [HttpGet]
        [Route("GetAllAuthorizersGroup")]
        public async Task<IActionResult> GetAllAuthorizersGroup()
        {
            List<AuthorizerGroupModel> authorizersGroup = new List<AuthorizerGroupModel>();

            authorizersGroup = await _authorizerGroupService.GetAllAuthorizerGroup();

            return Ok(authorizersGroup);
        }

        [HttpGet]
        [Route("GetAuthorizerGroup")]
        public async Task<IActionResult> GetAuthorizerGroup(string group_id)
        {
            List<AuthorizerGroupModel> _authorizerGroupModel = new List<AuthorizerGroupModel>();
            _authorizerGroupModel = await _authorizerGroupService.GetAuthorizerGroup(group_id);
            return Ok(_authorizerGroupModel);
        }

        [HttpPost]
        [Route("InsertGroup")]
        public async Task<IActionResult> InsertAuthorizerGroup(List<AuthorizerGroupModel> authorizerGroupModel)
        {
            var error_msg = await _authorizerGroupService.InsertAuthorizerGroupData(authorizerGroupModel);
            return Ok(error_msg);
        }
    }
}
