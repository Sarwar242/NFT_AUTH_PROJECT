using Microsoft.EntityFrameworkCore.Storage;
using ModelClasses;

namespace ServerSideApplication.Service.NftAuth;

public interface INftAuthService
{
    Task<bool> CreateNftLog(List<NftAuthModel> NftAutLogList, string tableName, IDbContextTransaction transaction);
}
