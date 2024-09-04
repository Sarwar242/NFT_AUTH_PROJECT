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
            try
            {
                await using (var command = _connection.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;


                    #region code added by monaem
                    /*
                    command.CommandText = "Packege_Monaem.unAuth_table_log_list";
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
                                //LOG_STATUS = reader["log_status"].ToString(),
                                TABLE_NAME = reader["table_name"].ToString(),
                                ACTION_STATUS = reader["action_status"].ToString(),
                                REMARKS = reader["remarks"].ToString(),
                                MAKE_BY = reader["make_by"].ToString()
                            });
                        }
                    }*/
                    #endregion


                    command.CommandText = "nft_auth_test_pck.nft_authorz_log_gk";

                    command.Parameters.Add(new OracleParameter("pbranch_id", OracleDbType.NVarchar2)
                    {
                        Direction = System.Data.ParameterDirection.Input,
                        Value = branch_id
                    });

                    command.Parameters.Add(new OracleParameter("puser_id", OracleDbType.NVarchar2)
                    {
                        Direction = System.Data.ParameterDirection.Input,
                        Value = name
                    });

                    command.Parameters.Add(new OracleParameter("pfunction", OracleDbType.NVarchar2)
                    {
                        Direction = System.Data.ParameterDirection.Input,
                        Value = functionId
                    });

                    command.Parameters.Add(new OracleParameter("plog_status", OracleDbType.NVarchar2)
                    {
                        Direction = System.Data.ParameterDirection.Input,
                        Value = "U"
                    });

                    command.Parameters.Add(new OracleParameter("psms_nft_authorz_log_cursor", OracleDbType.RefCursor)
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
                                LOG_STATUS = reader["log_status"].ToString(),
                                TABLE_NAME = reader["table_name"].ToString(),
                                ACTION_STATUS = reader["action_status"].ToString(),
                                REMARKS = reader["remarks"].ToString(),
                                MAKE_BY = reader["make_by"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex) {
                throw new Exception();
            }
            return nftAuthlogTableList;
        }


        #region Authorize/Decline
        public async Task<string> PostAuthDecline(AuthLogModel _logData,string _userName, string authStatus, string overrideflag, string designationOverride)
        {
            string error_message = "";
            string error_code;
            await using (var command = _connection.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "NFT_AUTH_TEST_PCK.nft_authorize_decline";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add(new OracleParameter("puser_id", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = _userName
                });
                command.Parameters.Add(new OracleParameter("pbranch_id", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = _logData.BRANCH_ID
                });
                command.Parameters.Add(new OracleParameter("pqueue_id", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = _logData.QUEUE_ID
                });
                command.Parameters.Add(new OracleParameter("pauthdecln", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = authStatus
                });
                command.Parameters.Add(new OracleParameter("puserid2", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = "",
                });
                command.Parameters.Add(new OracleParameter("premarks", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = _logData.REASON
                });
                command.Parameters.Add(new OracleParameter("poverride_flag", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = overrideflag
                });
                command.Parameters.Add(new OracleParameter("pdesig_override_auth_flag", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = designationOverride
                });

                var err_code = new OracleParameter("perrorcode", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(err_code);

                var err_msg = new OracleParameter("perrormsg", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(err_msg);

                await _connection.Database.OpenConnectionAsync();

                await command.ExecuteNonQueryAsync();

                error_message = err_msg.Value.ToString()??"";
                error_code = err_code.Value.ToString()??"";
            }

            return error_message;
        }
        #endregion
    }
}
