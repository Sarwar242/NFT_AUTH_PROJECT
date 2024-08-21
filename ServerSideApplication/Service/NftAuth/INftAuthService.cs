using ModelClasses.NftAuth;

namespace ServerSideApplication.Service.NftAuth;

public interface INftAuthService
{
    Task<bool> CreateNftLog(List<NftAuthModel> NftAutLogList, string tableName);
}
