using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses.NftAuth
{
    public class AuthLogModel
    {
        public Boolean isCheck {  get; set; } = false;
        public string? BRANCH_ID { get; set; }
        public string? QUEUE_ID { get; set; }
        public string? FUNCTION_ID { get; set; }
        public string? TABLE_NAME { get; set; }
        public string? ACTION_STATUS { get; set; }
        public string? MAKE_BY { get; set; }
    }
}
