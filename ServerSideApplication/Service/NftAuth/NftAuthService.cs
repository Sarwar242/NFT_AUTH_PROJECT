using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ModelClasses;
using ModelClasses.NftAuth;
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

    public async Task<bool> CreateNftLog(List<NftAuthModel> NftAutLogList, string tableName)
    {
        Guid qId = Guid.NewGuid();
        using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var primTable = true;
        var res = "null";
        try
        {
            foreach (var nftAuthModel in NftAutLogList)
            {

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "NFT_AUTH_TEST_PCK.cor_nft_auth_log_i";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transaction.GetDbTransaction();

                    var BRANCH_ID   = new OracleParameter("p_branch_id", OracleDbType.NVarchar2, 50) { Value = nftAuthModel.BRANCH_ID };
                    var QUEUE_ID        = new OracleParameter("p_queue_id", OracleDbType.NVarchar2) { Direction = ParameterDirection.InputOutput, Value = qId };
                    var FUNCTION_ID = new OracleParameter("p_function_id", OracleDbType.NVarchar2) { Value = nftAuthModel.FUNCTION_ID };
                    var TABLE_NAME = new OracleParameter("p_table_name", OracleDbType.NVarchar2) { Value = nftAuthModel.TABLE_NAME };
                    var TABLE_ROWID = new OracleParameter("p_table_rowid", OracleDbType.NVarchar2) { Value = nftAuthModel.TABLE_ROWID };
                    var COLUMN_NAME = new OracleParameter("p_column_name", OracleDbType.NVarchar2) { Value = nftAuthModel.COLUMN_NAME };
                    var DATA_TYPE = new OracleParameter("p_data_type", OracleDbType.NVarchar2) { Value = nftAuthModel.DATA_TYPE };
                    var OLD_VALUE = new OracleParameter("p_old_value", OracleDbType.NVarchar2) { Value = nftAuthModel.OLD_VALUE };
                    var NEW_VALUE = new OracleParameter("p_new_value", OracleDbType.NVarchar2) { Value = nftAuthModel.NEW_VALUE };
                    var ACTION_STATUS       = new OracleParameter("p_action_status", OracleDbType.NVarchar2) { Value = nftAuthModel.ACTION_STATUS };
                    var REMARKS             = new OracleParameter("p_remarks", OracleDbType.NVarchar2) { Value = nftAuthModel.REMARKS };
                    var MAKE_BY             = new OracleParameter("p_make_by", OracleDbType.NVarchar2) { Value = nftAuthModel.MAKE_BY };
                    var MAKE_DT = new OracleParameter("p_make_dt", OracleDbType.NVarchar2) { Value = nftAuthModel.MAKE_DT };
                    var PRIMARY_TABLE_FLAG  = new OracleParameter("p_primary_table_flag", OracleDbType.Int16) { Value = 0 };
                    if ((nftAuthModel.TABLE_NAME == tableName) && primTable)
                    {
                        PRIMARY_TABLE_FLAG = new OracleParameter("p_primary_table_flag", OracleDbType.Int16) { Value = 1 };
                        primTable = false;
                    }
                    var AUTH_1ST_BY         = new OracleParameter("p_auth_1st_by", OracleDbType.NVarchar2) { Value = nftAuthModel.AUTH_1ST_BY };
                    var AUTH_1ST_DT         = new OracleParameter("p_auth_1st_dt", OracleDbType.NVarchar2) { Value = nftAuthModel.AUTH_1ST_DT };
                    var AUTH_2ND_BY         = new OracleParameter("p_auth_2nd_by", OracleDbType.NVarchar2) { Value = nftAuthModel.AUTH_2ND_BY };
                    var AUTH_2ND_DT         = new OracleParameter("p_auth_2nd_dt", OracleDbType.NVarchar2) { Value = nftAuthModel.AUTH_2ND_DT };
                    var LOG_STATUS          = new OracleParameter("p_log_status", OracleDbType.NVarchar2) { Value = nftAuthModel.LOG_STATUS };
                    var REASON_DECLINE      = new OracleParameter("p_reason_decline", OracleDbType.NVarchar2) { Value = nftAuthModel.REASON_DECLINE };
                    var DISPLAY_LABEL       = new OracleParameter("p_display_label", OracleDbType.NVarchar2) { Value = nftAuthModel.DISPLAY_LABEL };
                    var DISPLAY_BLOCK       = new OracleParameter("p_display_block", OracleDbType.NVarchar2) { Value = nftAuthModel.DISPLAY_BLOCK };
                    var ALERT_FLAG          = new OracleParameter("p_alert_flag", OracleDbType.NVarchar2) { Value = nftAuthModel.ALERT_FLAG };
                    var Error_Msg           = new OracleParameter("p_error_msg", OracleDbType.Varchar2, 400) { Direction = ParameterDirection.Output };
                    var Error_Code          = new OracleParameter("p_error_code", OracleDbType.Varchar2, 20) { Direction = ParameterDirection.Output };
                    var RowId               = new OracleParameter("p_rowId", OracleDbType.Varchar2, 18) { Direction = ParameterDirection.Output };

                    command.Parameters.AddRange(new[] { BRANCH_ID, QUEUE_ID, FUNCTION_ID, TABLE_NAME, TABLE_ROWID, COLUMN_NAME, DATA_TYPE, OLD_VALUE, NEW_VALUE,
                        ACTION_STATUS, REMARKS, MAKE_BY, MAKE_DT, PRIMARY_TABLE_FLAG, AUTH_1ST_BY, AUTH_1ST_DT, AUTH_2ND_BY, AUTH_2ND_DT, LOG_STATUS, REASON_DECLINE,
                        DISPLAY_LABEL, DISPLAY_BLOCK, ALERT_FLAG, ACTION_STATUS, REMARKS, Error_Msg, Error_Code, RowId });

                    await _context.Database.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync();

                    if (Error_Code.Value != null && Error_Code.Value.ToString() != "null")
                    {
                        Console.WriteLine($"Error Code: {Error_Code.Value}, Error Message: {Error_Msg.Value}");
                        res = Error_Msg.Value?.ToString() ?? "null";
                        throw new Exception("Error Occured: "+ Error_Msg.Value);
                    }
                }
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"An error occurred: {ex.Message}");
            return await Task.FromResult(false);
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }

        return await Task.FromResult(true);
    }
}
