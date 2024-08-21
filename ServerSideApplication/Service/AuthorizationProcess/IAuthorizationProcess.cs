using ModelClasses;
using ModelClasses.NftAuth;

namespace ServerSideApplication.Service.AuthorizationProcess
{
    public interface IAuthorizationProcess
    {
        public Task<List<DesignationModel>> GetAllUserList();
        public Task<List<AuthLogModel>> GetAuthLogTableName(string name, string branch_id);
        public Task<List<AuthLogModel>> GetAuthLogTableDataList(string name, string branch_id, string functionId);
    }
}
