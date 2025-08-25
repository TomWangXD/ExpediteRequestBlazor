using ExpediteRequestBlazor.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpediteRequestBlazor.DataTransferObjects
{
    public class DocumentModel
    {
        public DocumentModel()
        {

        }
        public DocumentModel(Document doc)
        {
            Id = doc.Id;
            Gkey = doc.Gkey;
            CoNum = doc.CoNum;
            CoLine = doc.CoLine;
            CoRelease = doc.CoRelease;
            ShipSite = doc.ShipSite;
            Job = doc.Job;
            Ipn = doc.Ipn;
            IpnDescription = doc.IpnDescription;
            OrderDate = doc.OrderDate;
            CustomerName = doc.CustomerName;
            QtyOrdered = doc.QtyOrdered;
            Um = doc.Um;
            DueDate = doc.DueDate;
            ShelfLifeRequirementFlag = doc.ShelfLifeRequirementFlag;
            ShelfLifeRequirement = doc.ShelfLifeRequirement;
            PonderosaPack = doc.PonderosaPack;
            CurrentDueDate = doc.CurrentDueDate;
            PartialQuantityAccepted = doc.PartialQuantityAccepted;
            PartialQuantity = doc.PartialQuantity;
            ShipDate = doc.ShipDate;
            Fee = doc.Fee;
            Reason = doc.Reason;
            NewShipDate = doc.NewShipDate;
            NewShipType = doc.NewShipType;
            Status = doc.Status;
            Comments = doc.Comments;
            CreatedBy = doc.CreatedBy;
            Created = doc.Created;
            ModifiedBy = doc.ModifiedBy;
            Modified = doc.Modified;
            Approvals = doc.Approvals;
            Dpas = doc.Dpas;
            Snowshoe = doc.Snowshoe;
            InProgress = doc.InProgress;
            PlanCode = doc.PlanCode;
        }

        public int Id { get; set; }
        public Guid? Gkey { get; set; }
        public string CoNum { get; set; }
        public short CoLine { get; set; }
        public short CoRelease { get; set; }
        public string ShipSite { get; set; }
        public string Job { get; set; }
        public string Ipn { get; set; }
        public string IpnDescription { get; set; }
        public DateTime? OrderDate { get; set; }
        public string CustomerName { get; set; }
        public decimal? QtyOrdered { get; set; }
        public string Um { get; set; }
        public DateTime? DueDate { get; set; }
        public string? PlanCode { get; set; }
        public bool ShelfLifeRequirementFlag { get; set; }
        public string ShelfLifeRequirement { get; set; }
        public bool PonderosaPack { get; set; }
        public bool Dpas { get; set; }
        public bool Snowshoe { get; set; }
        public bool InProgress { get; set; }
        public bool CurrentDueDate { get; set; }
        public bool PartialQuantityAccepted { get; set; }
        public decimal PartialQuantity { get; set; }
        public DateTime? ShipDate { get; set; }
        public bool Fee { get; set; }
        public string Reason { get; set; }
        public DateTime? NewShipDate { get; set; }
        public string NewShipType { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Created { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime Modified { get; set; }
        public string Approver { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovalRemarks { get; set; }
        public DateTime? ApprovalTimestamp { get; set; }
        public int? ResponseTime
        {
            get
            {
                if (!ApprovalTimestamp.HasValue)
                {
                    return null;
                }

                TimeSpan timeSpan = ApprovalTimestamp.Value.Subtract(Created.Value);
                return ApprovalTimestamp == null ? null : (int)Math.Ceiling(timeSpan.TotalHours);
            }
        }
        public virtual ICollection<Approval> Approvals { get; set; }
    }
}
