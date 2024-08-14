using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using ServerSideApplication.DbConnection;
using ModelClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerSideApplication.Service
{
    public class GroceryService : IGroceryService
    {
        private readonly AppDbConnection _connection;

        public GroceryService(AppDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<string> PostGroceryListData(GroceryList _groceryList)
        {
            string error_msg = "null";
            string error_code = "";
            string rowid = "";

            await using (var transaction = await _connection.Database.BeginTransactionAsync())
            {
                await using (var command = _connection.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Packege_Monaem.Grocery_List_I";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

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
                        Size = 2000 // Adjust size according to expected length
                    };
                    command.Parameters.Add(err_msg);

                    var err_code = new OracleParameter("p_error_code", OracleDbType.NVarchar2)
                    {
                        Direction = System.Data.ParameterDirection.Output,
                        Size = 2000 // Adjust size according to expected length
                    };
                    command.Parameters.Add(err_code);

                    var v_rowid = new OracleParameter("v_rowId", OracleDbType.NVarchar2)
                    {
                        Direction = System.Data.ParameterDirection.Output,
                        Size = 2000 // Adjust size according to expected length
                    };
                    command.Parameters.Add(v_rowid);

                    await command.ExecuteNonQueryAsync();

                    error_msg = err_msg.Value?.ToString() ?? "null";
                    error_code = err_code.Value?.ToString() ?? "";
                    rowid = v_rowid.Value?.ToString() ?? "";

                    if (!string.IsNullOrEmpty(error_msg) && error_msg != "null")
                    {
                        await transaction.RollbackAsync();
                    }
                    else
                    {
                        await transaction.CommitAsync();
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
