using Microsoft.EntityFrameworkCore;
using ModelClasses;
using ModelClasses.AuthorizerGroup;
using Oracle.ManagedDataAccess.Client;
using ServerSideApplication.DbConnection;

namespace ServerSideApplication.Service.AuthorizerGroup
{
    public class AuthorizerGroupService:IAuthorizerGroupService
    {
        private readonly AppDbConnection _connection;

        public AuthorizerGroupService(AppDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<string>> GetAllDesignation()
        {
            List<string> designation = new List<string>();
            await using (var command = _connection.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "Packege_Monaem.cor_designation_ga";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add(new OracleParameter("p_sys_cursor", OracleDbType.RefCursor)
                {
                    Direction = System.Data.ParameterDirection.Output
                });

                await _connection.Database.OpenConnectionAsync();

                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        designation.Add(reader["designation_name"].ToString());
                    }
                }
            }
            return designation;
        }

        public async Task<List<AuthorizerGroupModel>> GetAllAuthorizerGroup()
        {
            List<AuthorizerGroupModel> authorizersGroup = new List<AuthorizerGroupModel>();

            await using (var command = _connection.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "Packege_Monaem.COR_NFT_AUTHORIZER_GROUP_GA";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                
                var error_msg = new OracleParameter("p_error_msg", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(error_msg);

                var error_code = new OracleParameter("p_error_code", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(error_code);

                command.Parameters.Add(new OracleParameter("p_sys_cursor", OracleDbType.RefCursor)
                {
                    Direction = System.Data.ParameterDirection.Output
                });

                await _connection.Database.OpenConnectionAsync();

                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        authorizersGroup.Add( new AuthorizerGroupModel
                        {
                            group_id = Convert.ToInt32(reader["group_id"].ToString()),
                            group_nm = reader["group_nm"].ToString()
                        });
                    }
                }
            }
            return authorizersGroup;
        }

        public async Task<List<AuthorizerGroupModel>> GetAuthorizerGroup(string _athorizerGroupId)
        {
            List<AuthorizerGroupModel> authorizerGroup = new List<AuthorizerGroupModel>();

            await using (var command = _connection.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "Packege_Monaem.COR_NFT_AUTHORIZER_GROUP_GI";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add(new OracleParameter("p_group_id", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Input,
                    Value = _athorizerGroupId
                });

                var error_msg = new OracleParameter("p_error_msg", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(error_msg);

                var error_code = new OracleParameter("p_error_code", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(error_code);

                command.Parameters.Add(new OracleParameter("p_sys_cursor", OracleDbType.RefCursor)
                {
                    Direction = System.Data.ParameterDirection.Output
                });

                await _connection.Database.OpenConnectionAsync();

                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        authorizerGroup.Add(new AuthorizerGroupModel
                        {
                            group_id = Convert.ToInt32(reader["group_id"].ToString()),
                            group_nm = reader["group_nm"].ToString(),
                            designation_nm = reader["designation_name"].ToString(),
                            desig_override_auth_flag = Convert.ToInt32(reader["desig_override_auth_flag"].ToString()),
                            make_by = reader["make_by"].ToString(),
                            authrizer_sl_no = Convert.ToInt32(reader["authrizer_sl_no"].ToString())
                        });
                    }
                }
            }
            return authorizerGroup;
        }

        public async Task<string> InsertAuthorizerGroupData(List<AuthorizerGroupModel> _authorizerGroupModel)
        {
            string error_msg = "null";
            string error_code = "";
            int v_group_id = 0;

            await using (var transaction = await _connection.Database.BeginTransactionAsync())
            {
                foreach (var authorizerGroupModel in _authorizerGroupModel)
                {
                    if(authorizerGroupModel.last_action == "ADD")
                    {
                        if (string.IsNullOrEmpty(authorizerGroupModel.group_id.ToString()))
                        {
                            authorizerGroupModel.group_id = v_group_id;
                        }

                        await using (var command = _connection.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = "Packege_Monaem.COR_NFT_AUTHORIZER_GROUP_I";
                            command.CommandType = System.Data.CommandType.StoredProcedure;

                            var group_id = new OracleParameter("p_group_id", OracleDbType.NVarchar2)
                            {
                                Direction = System.Data.ParameterDirection.InputOutput,
                                Value = authorizerGroupModel.group_id
                            };
                            command.Parameters.Add(group_id);

                            command.Parameters.Add(new OracleParameter("p_group_nm", OracleDbType.NVarchar2)
                            {
                                Direction = System.Data.ParameterDirection.Input,
                                Value = authorizerGroupModel.group_nm
                            });

                            command.Parameters.Add(new OracleParameter("p_designation_nm", OracleDbType.NVarchar2)
                            {
                                Direction = System.Data.ParameterDirection.Input,
                                Value = authorizerGroupModel.designation_nm
                            });

                            command.Parameters.Add(new OracleParameter("p_authrizer_sl_no", OracleDbType.NVarchar2)
                            {
                                Direction = System.Data.ParameterDirection.Input,
                                Value = authorizerGroupModel.authrizer_sl_no
                            });

                            command.Parameters.Add(new OracleParameter("p_desig_override_auth_flag", OracleDbType.NVarchar2)
                            {
                                Direction = System.Data.ParameterDirection.Input,
                                Value = authorizerGroupModel.desig_override_auth_flag
                            });

                            command.Parameters.Add(new OracleParameter("p_make_by", OracleDbType.NVarchar2)
                            {
                                Direction = System.Data.ParameterDirection.Input,
                                Value = authorizerGroupModel.make_by
                            });

                            command.Parameters.Add(new OracleParameter("p_last_action", OracleDbType.NVarchar2)
                            {
                                Direction = System.Data.ParameterDirection.Input,
                                Value = authorizerGroupModel.last_action
                            });

                            var err_msg = new OracleParameter("p_error_msg", OracleDbType.NVarchar2)
                            {
                                Direction = System.Data.ParameterDirection.Output,
                                Size = 2000 // Adjust size according to expected length
                            };
                            command.Parameters.Add(err_msg);

                            var err_code = new OracleParameter("p_error_code", OracleDbType.NVarchar2)
                            {
                                Direction = System.Data.ParameterDirection.Output,
                                Size = 2000 // Adjust size according to expected length
                            };
                            command.Parameters.Add(err_code);

                            await command.ExecuteNonQueryAsync();

                            v_group_id = Convert.ToInt32(group_id.Value?.ToString());
                            error_msg = err_msg.Value?.ToString() ?? "null";
                            error_code = err_code.Value?.ToString() ?? "";

                            if (!string.IsNullOrEmpty(error_msg) && error_msg != "null")
                            {
                                break;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(error_msg) && error_msg != "null")
                {
                    await transaction.RollbackAsync();
                }
                else
                {
                    await transaction.CommitAsync();
                }
            }

            return error_msg;
        }
    }
}
