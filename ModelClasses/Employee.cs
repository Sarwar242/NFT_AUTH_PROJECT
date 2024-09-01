using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses
{
    public class Employee
    {
        public string? EMPLOYEE_ID { get; set; }
        public string? EMPLOYEE_NAME { get; set; }
        public string? BRANCH_ID { get; set; }
        public string? DEPARTMENT { get; set; }
        public string? DESIGNATION_NAME { get; set; }
        public int? DESIGNATION_ID  { get; set; }
        public DateTime JOINING_DT { get; set; }
        public bool ACTIVE_FLAG { get;set; }
    }
}
