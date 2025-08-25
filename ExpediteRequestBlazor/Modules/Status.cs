using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpediteRequestBlazor.Models
{
    public static class Status
    {
        public const string SUBMITTED = "Submitted";
        public const string AWAITING_PRODUCTION = "Awaiting Production";
        public const string APPROVE_TOTAL = "Approve Total";
        public const string APPROVE_PARTIAL = "Approve Partial";
        public const string DATE_MODIFIED_TOTAL = "Date Modified - Total";
        public const string DATE_MODIFIED_PARTIAL = "Date Modified - Partial";
        public const string DECLINED = "Declined";
        public const string COMPLETE = "Complete";
    }
}
