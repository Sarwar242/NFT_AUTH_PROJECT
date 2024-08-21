using ModelClasses.NftAuth;

namespace ServerSideApplication.Service.NftAuth;

public interface INftAuthService
{
    Task CreateNftLog(List<NftAuthModel> NftAutLogList, string tableName);
}
