

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ModelClasses;
using Oracle.ManagedDataAccess.Client;
using ServerSideApplication.DbConnection;
using System.Data;

namespace ServerSideApplication.Service.NftAuth;

public class NftAuthService : INftAuthService
{
    private readonly AppDbConnection _context;
    public NftAuthService(AppDbConnection appDbConnection)
    {
        _context = appDbConnection;
    }

    public async Task<bool> CreateNftLog(List<NftAuthModel> NftAutLogList, string tableName, IDbContextTransaction transaction2)
    {
        var primTable = true;
        var res = "null";
        var transaction = transaction2;
        try
        {
            foreach (var nftAuthModel in NftAutLogList)
            {
                var command2 = _context.Database.GetDbConnection().CreateCommand();
                command2.Transaction = transaction.GetDbTransaction();
                command2.CommandText = "NFT_AUTH_TEST_PCK.cor_nft_auth_log_i";
                command2.CommandType = CommandType.StoredProcedure;

                command2.Parameters.Add(new OracleParameter("p_branch_id", OracleDbType.NVarchar2, 50) { Value = nftAuthModel.BRANCH_ID });
                command2.Parameters.Add(new OracleParameter("p_queue_id", OracleDbType.NVarchar2) { Direction = ParameterDirection.InputOutput, Value = Guid.NewGuid() });
                command2.Parameters.Add(new OracleParameter("p_function_id", OracleDbType.NVarchar2) { Value = nftAuthModel.FUNCTION_ID });
                command2.Parameters.Add(new OracleParameter("p_table_name", OracleDbType.NVarchar2) { Value = nftAuthModel.TABLE_NAME });
                command2.Parameters.Add(new OracleParameter("p_table_rowid", OracleDbType.NVarchar2) { Value = nftAuthModel.TABLE_ROWID });
                command2.Parameters.Add(new OracleParameter("p_column_name", OracleDbType.NVarchar2) { Value = nftAuthModel.COLUMN_NAME });
                command2.Parameters.Add(new OracleParameter("p_data_type", OracleDbType.NVarchar2) { Value = nftAuthModel.DATA_TYPE });
                command2.Parameters.Add(new OracleParameter("p_old_value", OracleDbType.NVarchar2) { Value = nftAuthModel.OLD_VALUE });
                command2.Parameters.Add(new OracleParameter("p_new_value", OracleDbType.NVarchar2) { Value = nftAuthModel.NEW_VALUE });
                command2.Parameters.Add(new OracleParameter("p_action_status", OracleDbType.NVarchar2) { Value = nftAuthModel.ACTION_STATUS });
                command2.Parameters.Add(new OracleParameter("p_remarks", OracleDbType.NVarchar2) { Value = nftAuthModel.REMARKS });
                command2.Parameters.Add(new OracleParameter("p_make_by", OracleDbType.NVarchar2) { Value = nftAuthModel.MAKE_BY });
                command2.Parameters.Add(new OracleParameter("p_make_dt", OracleDbType.NVarchar2) { Value = nftAuthModel.MAKE_DT });
                var primaryTableFlag = (nftAuthModel.TABLE_NAME == tableName && primTable) ? 1 : 0;
                command2.Parameters.Add(new OracleParameter("p_primary_table_flag", OracleDbType.Int16) { Value = primaryTableFlag });
                primTable = false;

                command2.Parameters.Add(new OracleParameter("p_auth_1st_by", OracleDbType.NVarchar2) { Value = nftAuthModel.AUTH_1ST_BY });
                command2.Parameters.Add(new OracleParameter("p_auth_1st_dt", OracleDbType.NVarchar2) { Value = nftAuthModel.AUTH_1ST_DT });
                command2.Parameters.Add(new OracleParameter("p_auth_2nd_by", OracleDbType.NVarchar2) { Value = nftAuthModel.AUTH_2ND_BY });
                command2.Parameters.Add(new OracleParameter("p_auth_2nd_dt", OracleDbType.NVarchar2) { Value = nftAuthModel.AUTH_2ND_DT });
                command2.Parameters.Add(new OracleParameter("p_log_status", OracleDbType.NVarchar2) { Value = nftAuthModel.LOG_STATUS });
                command2.Parameters.Add(new OracleParameter("p_reason_decline", OracleDbType.NVarchar2) { Value = nftAuthModel.REASON_DECLINE });
                command2.Parameters.Add(new OracleParameter("p_display_label", OracleDbType.NVarchar2) { Value = nftAuthModel.DISPLAY_LABEL });
                command2.Parameters.Add(new OracleParameter("p_display_block", OracleDbType.NVarchar2) { Value = nftAuthModel.DISPLAY_BLOCK });
                command2.Parameters.Add(new OracleParameter("p_alert_flag", OracleDbType.NVarchar2) { Value = nftAuthModel.ALERT_FLAG });
                command2.Parameters.Add(new OracleParameter("p_error_msg", OracleDbType.Varchar2, 400) { Direction = ParameterDirection.Output });
                command2.Parameters.Add(new OracleParameter("p_error_code", OracleDbType.Varchar2, 20) { Direction = ParameterDirection.Output });
                command2.Parameters.Add(new OracleParameter("p_rowId", OracleDbType.Varchar2, 18) { Direction = ParameterDirection.Output });

                await command2.ExecuteNonQueryAsync();

                if (command2.Parameters["p_error_code"].Value != null && command2.Parameters["p_error_code"].Value.ToString() != "null")
                {
                    Console.WriteLine($"Error Code: {command2.Parameters["p_error_code"].Value}, Error Message: {command2.Parameters["p_error_msg"].Value}");
                    res = command2.Parameters["p_error_msg"].Value?.ToString() ?? "null";
                    throw new Exception("Error Occurred: " + command2.Parameters["p_error_msg"].Value);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }
}
