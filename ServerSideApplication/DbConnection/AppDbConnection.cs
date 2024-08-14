using Microsoft.EntityFrameworkCore;

namespace ServerSideApplication.DbConnection
{
    public class AppDbConnection : DbContext
    {
        public AppDbConnection(DbContextOptions<AppDbConnection> options) : base(options) { }
    }
}
