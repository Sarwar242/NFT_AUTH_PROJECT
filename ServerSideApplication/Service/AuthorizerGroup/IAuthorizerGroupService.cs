using ModelClasses.AuthorizerGroup;

namespace ServerSideApplication.Service.AuthorizerGroup
{
    public interface IAuthorizerGroupService
    {
        Task<List<string>> GetAllDesignation();
        Task<List<AuthorizerGroupModel>> GetAllAuthorizerGroup();
        Task<List<AuthorizerGroupModel>> GetAuthorizerGroup(string _athorizerGroupId);
        Task<string> InsertAuthorizerGroupData(List<AuthorizerGroupModel> _authorizerGroupModel);
    }
}
