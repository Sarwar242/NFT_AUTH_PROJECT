using Microsoft.EntityFrameworkCore;
using ModelClasses;
using ModelClasses.NftAuth;
using Oracle.ManagedDataAccess.Client;
using ServerSideApplication.DbConnection;

namespace ServerSideApplication.Service.AuthorizationProcess
{
    public class AuthorizationProcess:IAuthorizationProcess
    {
        private readonly AppDbConnection _connection;

        public AuthorizationProcess(AppDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<DesignationModel>> GetAllUserList()
        {
            List<DesignationModel> list = new List<DesignationModel>();
            await using (var command = _connection.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "Packege_Monaem.get_All_Employe";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor)
                {
                    Direction = System.Data.ParameterDirection.Output
                });

                await _connection.Database.OpenConnectionAsync();

                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new DesignationModel
                        {
                            Designation_Id = reader["employee_name"].ToString(),
                            Designation_Name = reader["details_"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public async Task<List<AuthLogModel>> GetAuthLogTableName(string name,string branch_id)
        {
            List<AuthLogModel> nftAuthlogTableList = new List<AuthLogModel>();
            await using (var command = _connection.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "Packege_Monaem.unAuth_table_list_ga";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add(new OracleParameter("p_branch_id", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value=branch_id
                });

                command.Parameters.Add(new OracleParameter("p_user_id", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value=name
                });

                command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor)
                {
                    Direction = System.Data.ParameterDirection.Output
                });

                await _connection.Database.OpenConnectionAsync();

                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        nftAuthlogTableList.Add(new AuthLogModel
                        {
                            TABLE_NAME = reader["table_name"].ToString(),
                            FUNCTION_ID = reader["function_id"].ToString()
                        });
                    }
                }
            }
            return nftAuthlogTableList;
        }


        public async Task<List<AuthLogModel>> GetAuthLogTableDataList(string name, string branch_id,string functionId)
        {
            List<AuthLogModel> nftAuthlogTableList = new List<AuthLogModel>();
            await using (var command = _connection.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "Packege_Monaem.unAuth_table_log_list";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add(new OracleParameter("p_branch_id", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = branch_id
                });

                command.Parameters.Add(new OracleParameter("p_user_id", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = name
                });

                command.Parameters.Add(new OracleParameter("p_function_id", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = functionId
                });

                command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor)
                {
                    Direction = System.Data.ParameterDirection.Output
                });

                await _connection.Database.OpenConnectionAsync();

                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        nftAuthlogTableList.Add(new AuthLogModel
                        {
                            BRANCH_ID = reader["branch_id"].ToString(),
                            QUEUE_ID = reader["queue_id"].ToString(),
                            FUNCTION_ID = reader["function_id"].ToString(),
                            TABLE_NAME = reader["table_name"].ToString(),
                            ACTION_STATUS = reader["action_status"].ToString(),
                            MAKE_BY = reader["make_by"].ToString()
                        });
                    }
                }
            }
            return nftAuthlogTableList;
        }
    }
}
