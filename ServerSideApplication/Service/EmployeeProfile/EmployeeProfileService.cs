using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ModelClasses;
using Oracle.ManagedDataAccess.Client;
using ServerSideApplication.DbConnection;
using System.Data;

namespace ServerSideApplication.Service.EmployeeProfile
{
    public class EmployeeProfileService : IEmployeeProfileService
    {
        private readonly AppDbConnection _Context;
        public EmployeeProfileService(AppDbConnection Context)
        {
            _Context = Context;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            List<Employee> EmployeeList = new List<Employee>();

            using (var command = _Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "NFT_AUTH_TEST_PCK.EMPLOYEE_PROFILR_GA";
                command.CommandType = CommandType.StoredProcedure;

                var cursorParam = new OracleParameter("p_sys_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };
                command.Parameters.Add(cursorParam);

                try
                {
                    await _Context.Database.OpenConnectionAsync();

                    using (var result = await command.ExecuteReaderAsync())
                    {
                        while (await result.ReadAsync())
                        {
                            EmployeeList.Add(new Employee
                            {
                                EMPLOYEE_ID = result["EMPLOYEE_ID"].ToString(),
                                EMPLOYEE_NAME = result["EMPLOYEE_NAME"].ToString(),
                                BRANCH_ID = result["BRANCH_ID"].ToString(),
                                DEPARTMENT = result["DEPARTMENT"].ToString(),
                                DESIGNATION_ID = Convert.ToInt32(result["DESIGNATION_ID"].ToString()),
                                JOINING_DT = Convert.ToDateTime(result["JOINING_DT"].ToString()),
                                ACTIVE_FLAG = Convert.ToBoolean(result["ACTIVE_FLAG"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while fetching Employee Information: {ex.Message}");
                }
                finally
                {
                    await _Context.Database.CloseConnectionAsync();
                }
            }

            return EmployeeList;
        }

        public async Task<string> CreateEmployee(Employee EmployeeModel)
        {
            using var transaction = await _Context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var res = "null";
            try
            {
              
                using (var command = _Context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "NFT_AUTH_TEST_PCK.Employee_Profile_I";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transaction.GetDbTransaction();

                    var EMPLOYEE_ID = new OracleParameter("P_EMPLOYEE_ID", OracleDbType.Varchar2,50) { Direction = ParameterDirection.Output };
                    var EMPLOYEE_NAME = new OracleParameter("P_EMPLOYEE_NAME", OracleDbType.Varchar2) { Value = EmployeeModel.EMPLOYEE_NAME };
                    var BRANCH_ID = new OracleParameter("P_BRANCH_ID", OracleDbType.Varchar2) { Value = EmployeeModel.BRANCH_ID };
                    var DEPARTMENT = new OracleParameter("P_DEPARTMENT", OracleDbType.Varchar2) { Value = EmployeeModel.DEPARTMENT};
                    var DESIGNATION_ID = new OracleParameter("P_DESIGNATION_ID", OracleDbType.Int32) { Value = EmployeeModel.DESIGNATION_ID };
                    var JOINING_DT = new OracleParameter("P_JOINING_DT", OracleDbType.Date) { Value =EmployeeModel.JOINING_DT };
                    var ACTIVE_FLAG = new OracleParameter("P_ACTIVE_FLAG", OracleDbType.Int32) { Value = EmployeeModel.ACTIVE_FLAG };
                    var Error_Msg = new OracleParameter("p_error_msg", OracleDbType.Varchar2, 400) { Direction = ParameterDirection.Output };
                    var Error_Code = new OracleParameter("p_error_code", OracleDbType.Varchar2, 20) { Direction = ParameterDirection.Output };
                    var RowId = new OracleParameter("p_rowId", OracleDbType.Varchar2, 18) { Direction = ParameterDirection.Output };

                    command.Parameters.AddRange(new[] { EMPLOYEE_ID, EMPLOYEE_NAME, BRANCH_ID, DEPARTMENT, DESIGNATION_ID, JOINING_DT, ACTIVE_FLAG, Error_Msg, Error_Code, RowId });

                    await _Context.Database.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync();

                    if (Error_Code.Value != null && Error_Code.Value.ToString() != "null")
                    {
                        Console.WriteLine($"Error Code: {Error_Code.Value}, Error Message: {Error_Msg.Value}");
                        res = Error_Msg.Value?.ToString() ?? "null";
                        return res;
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
                await _Context.Database.CloseConnectionAsync();
            }
        }

    }
}
