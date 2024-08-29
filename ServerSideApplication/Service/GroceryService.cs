using Microsoft.EntityFrameworkCore;
using ServerSideApplication.DbConnection;
using ModelClasses;
using ServerSideApplication.Service.NftAuth;
using Oracle.ManagedDataAccess.Client;

namespace ServerSideApplication.Service
{
    public class GroceryService : IGroceryService
    {
        private readonly AppDbConnection _connection;
        private readonly INftAuthService _nftAuthService;

        public GroceryService(AppDbConnection connection, INftAuthService nftAuthService)
        {
            _connection = connection;
            _nftAuthService = nftAuthService;
        }

        public async Task<string> PostGroceryListData(GroceryList _groceryList)
        {
            string error_msg = "null";
            string error_code = "";
            string rowid = "";
            string item_id = "";
            var nftAuthLogList = new List<NftAuthModel>();

            var transaction = await _connection.Database.BeginTransactionAsync();
            
                await using (var command = _connection.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Packege_Monaem.Grocery_List_I";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                var grocery_id = new OracleParameter("p_grocery_id", OracleDbType.NVarchar2)
                {
                    Direction = System.Data.ParameterDirection.Output,
                    Size = 2000
                };

                    command.Parameters.Add(grocery_id);

                command.Parameters.Add(new OracleParameter("p_grocery_name", OracleDbType.NVarchar2)
                    {
                        Direction = System.Data.ParameterDirection.Input,
                        Value = _groceryList.Item_Name
                    });

                    command.Parameters.Add(new OracleParameter("p_grocery_price", OracleDbType.NVarchar2)
                    {
                        Direction = System.Data.ParameterDirection.Input,
                        Value = _groceryList.Item_Price
                    });

                    command.Parameters.Add(new OracleParameter("p_make_by", OracleDbType.NVarchar2)
                    {
                        Direction = System.Data.ParameterDirection.Input,
                        Value = _groceryList.Make_By
                    });

                    command.Parameters.Add(new OracleParameter("p_last_action", OracleDbType.NVarchar2)
                    {
                        Direction = System.Data.ParameterDirection.Input,
                        Value = _groceryList.Action_Status
                    });

                    var err_msg = new OracleParameter("p_error_msg", OracleDbType.NVarchar2)
                    {
                        Direction = System.Data.ParameterDirection.Output,
                        Size = 2000
                    };
                    command.Parameters.Add(err_msg);

                    var err_code = new OracleParameter("p_error_code", OracleDbType.NVarchar2)
                    {
                        Direction = System.Data.ParameterDirection.Output,
                        Size = 20
                    };
                    command.Parameters.Add(err_code);

                    var v_rowid = new OracleParameter("v_rowId", OracleDbType.NVarchar2)
                    {
                        Direction = System.Data.ParameterDirection.Output,
                        Size = 200
                    };
                    command.Parameters.Add(v_rowid);

                    await command.ExecuteNonQueryAsync();

                    error_msg = err_msg.Value?.ToString() ?? "null";
                    error_code = err_code.Value?.ToString() ?? "";
                    rowid = v_rowid.Value?.ToString() ?? "";
                    item_id = grocery_id.Value?.ToString()??"";

                    if (!string.IsNullOrEmpty(error_msg) && error_msg != "null")
                    {
                        await transaction.RollbackAsync();
                    }
                    else
                    {
                        var authLog = new NftAuthModel();
                        authLog.TABLE_NAME = "GROCERY_LIST_TABLE";
                        authLog.TABLE_ROWID = rowid;
                        authLog.COLUMN_NAME = "";
                        authLog.OLD_VALUE = "";
                        authLog.NEW_VALUE = "";
                        authLog.PRIMARY_TABLE_FLAG = true;
                        authLog.FUNCTION_ID = "10201";
                        authLog.DATA_TYPE = "NVARCHAR2";
                        authLog.BRANCH_ID = "0001";
                        authLog.MAKE_BY = _groceryList.Make_By;
                        authLog.ACTION_STATUS = "ADD";
                        authLog.REMARKS = "New Grocery Added. Id: "+ item_id;

                        nftAuthLogList.Add(authLog);
                        var stat = await _nftAuthService.CreateNftLog(nftAuthLogList, "GROCERY_LIST_TABLE", transaction);
                        if (stat)
                        {
                            await transaction.CommitAsync();
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                        }
                        
                    }
                }

            return error_msg;
        }

        public async Task<List<GroceryList>> GetGroceryData()
        {
            List<GroceryList> _groceryList = new List<GroceryList>();

            await using (var command = _connection.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "Packege_Monaem.Grocery_List_GA";
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
                        _groceryList.Add(new GroceryList
                        {
                            Item_Id = reader["grocery_id"].ToString(),
                            Item_Name = reader["grocery_name"].ToString(),
                            Item_Price = Convert.ToDouble(reader["grocery_price"].ToString()),
                            Make_By = reader["make_by"].ToString(),
                            Action_Status = reader["last_action"].ToString()
                        });
                    }
                }
            }

            return _groceryList;
        }
    }
}
