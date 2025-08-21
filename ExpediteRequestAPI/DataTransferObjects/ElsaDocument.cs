using ExpediteRequestAPI.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpediteRequestAPI.DataTransferObjects
{
    public class ElsaDocument
    {
        public ElsaDocument(Document document)
        {
            Document = document;
            ProductionApproval = document.Approvals?.FirstOrDefault();
        }

        public Document Document { get; set; }
        public Approval? ProductionApproval { get; set; }

        public string EmailAPI { get; set; }
        public string OrigninatorEmail { get; set; }
        public string AppLocation { get; set; }
    }
}
