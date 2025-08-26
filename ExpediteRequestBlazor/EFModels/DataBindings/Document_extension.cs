using ExpediteRequestBlazor.DataTransferObjects;

namespace ExpediteRequestBlazor.EFModels
{
    public partial class Document
    {

        public Document(Document doc, SytelineDocumentData sytelineData)
        {
            CoNum = doc.CoNum;
            CoLine = doc.CoLine;
            CoRelease = doc.CoRelease;
            ShipSite = sytelineData.ShipSite;
            Job = sytelineData.Job;
            Ipn = sytelineData.Ipn;
            IpnDescription = sytelineData.IpnDescription;
            OrderDate = sytelineData.OrderDate;
            CustomerName = sytelineData.CustomerName;
            QtyOrdered = sytelineData.QtyOrdered;
            Um = sytelineData.Um;
            DueDate = sytelineData.DueDate;
            PlanCode = sytelineData.PlanCode;
            ShipDate = doc.ShipDate;
        }

        public Document(DocumentModel doc)
        {
            Id = doc.Id;
            Gkey = doc.Gkey ?? Guid.Empty;
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
            ShipDate = doc.ShipDate ?? DateTime.UtcNow;
            Fee = doc.Fee;
            Reason = doc.Reason;
            NewShipDate = doc.NewShipDate;
            NewShipType = doc.NewShipType;
            Status = doc.Status;
            Comments = doc.Comments;
            CreatedBy = doc.CreatedBy;
            Created = doc.Created ?? DateTime.UtcNow;
            ModifiedBy = doc.ModifiedBy;
            Modified = doc.Modified;
            Approvals = doc.Approvals;
            Dpas = doc.Dpas;
            Snowshoe = doc.Snowshoe;
            InProgress = doc.InProgress;
            PlanCode = doc.PlanCode;
        }
    }
}
