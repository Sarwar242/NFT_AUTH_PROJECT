using Oracle.ManagedDataAccess.Client;
using System.Data;
using Microsoft.EntityFrameworkCore;
using ModelClasses;
using ServerSideApplication.DbConnection;
using Microsoft.EntityFrameworkCore.Storage;

namespace ServerSideApplication.Service;

public class DesignationService : IDesignationService
{
    private readonly AppDbConnection _context;
    public DesignationService(AppDbConnection context)
    {
        _context = context;
    }

    public async Task<List<DesignationModel>> GetAllAsync()
    {
        List<DesignationModel> designations = new List<DesignationModel>();

        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "NFT_AUTH_TEST_PCK.Designations_GA";
            command.CommandType = CommandType.StoredProcedure;

            var cursorParam = new OracleParameter("p_sys_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };
            command.Parameters.Add(cursorParam);

            try
            {
                await _context.Database.OpenConnectionAsync();

                using (var result = await command.ExecuteReaderAsync())
                {
                    while (await result.ReadAsync())
                    {
                        designations.Add(new DesignationModel
                        {
                            Designation_Id = result["designation_id"].ToString()!,
                            Designation_Name = result["designation_name"].ToString(),
                            Designation_Priority = result["designation_priority"].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching designations: {ex.Message}");
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        return designations;
    }


    public async Task<string> CreateAsync(DesignationModel designationModel)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var res = "null";
        try
        {
            var designation = designationModel;
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "NFT_AUTH_TEST_PCK.FSP_Designation_I";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaction.GetDbTransaction();

                var Designation_Id          = new OracleParameter("p_designation_id", OracleDbType.Varchar2, 50) { Direction = ParameterDirection.Output };
                var Designation_Name        = new OracleParameter("p_designation_name", OracleDbType.Varchar2) { Value = designation.Designation_Name };
                var Designation_Priority    = new OracleParameter("p_designation_priority", OracleDbType.Varchar2) { Value = designation.Designation_Priority };
                var Error_Msg               = new OracleParameter("p_error_msg", OracleDbType.Varchar2, 400) { Direction = ParameterDirection.Output };
                var Error_Code              = new OracleParameter("p_error_code", OracleDbType.Varchar2, 20) { Direction = ParameterDirection.Output };
                var RowId                   = new OracleParameter("p_rowId", OracleDbType.Varchar2, 18) { Direction = ParameterDirection.Output };

                command.Parameters.AddRange(new[] { Designation_Id, Designation_Name, Designation_Priority, Error_Msg, Error_Code, RowId });

                await _context.Database.OpenConnectionAsync();
                await command.ExecuteNonQueryAsync();

                if (Error_Code.Value != null && Error_Code.Value.ToString() != "null")
                {
                    Console.WriteLine($"Error Code: {Error_Code.Value}, Error Message: {Error_Msg.Value}");
                    res = Error_Msg.Value?.ToString() ?? "null";
                    return res;
                }

                if (Designation_Id.Value != null)
                {
                    designation.Designation_Id = Designation_Id.Value.ToString()!;
                }
               
            }
            
            await transaction.CommitAsync();

            return res;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"An error occurred: {ex.Message}");
            return ex.Message;
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }
    }
}
