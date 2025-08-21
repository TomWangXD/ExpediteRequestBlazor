using System;
using System.Collections.Generic;

namespace ExpediteRequestAPI.EFModels
{
    public partial class Approval
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Signature { get; set; }
        public DateTime? Date { get; set; }
        public string Remarks { get; set; }
        public int? DocumentId { get; set; }
        public string ApprovalType { get; set; }
        public string ApprovalStatus { get; set; }
    }
}
