using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Indium.Common.DataTransferObjects
{
    public class ActionItem
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string AssignedTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
    }
}
