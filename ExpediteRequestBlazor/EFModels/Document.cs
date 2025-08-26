using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpediteRequestBlazor.EFModels
{
    public partial class Document
    {
        public Document()
        {
            Approvals = new HashSet<Approval>();
        }

        public int Id { get; set; }
        public Guid Gkey { get; set; }
        public string DocumentNo { get; set; }
        public string CoNum { get; set; }
        public short CoLine { get; set; }
        public short CoRelease { get; set; }
        public bool ShelfLifeRequirementFlag { get; set; }
        public string ShelfLifeRequirement { get; set; }
        public bool PonderosaPack { get; set; }
        public bool CurrentDueDate { get; set; }
        public bool PartialQuantityAccepted { get; set; }
        public decimal PartialQuantity { get; set; }
        public DateTime ShipDate { get; set; }
        public bool Fee { get; set; }
        [Required]
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime? NewShipDate { get; set; }
        public string NewShipType { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime Modified { get; set; }
        public string ShipSite { get; set; }
        public string Job { get; set; }
        public string Ipn { get; set; }
        public string IpnDescription { get; set; }
        public DateTime? OrderDate { get; set; }
        public string CustomerName { get; set; }
        public decimal? QtyOrdered { get; set; }
        public string Um { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Dpas { get; set; }
        public bool Snowshoe { get; set; }
        public bool InProgress { get; set; }
        public string PlanCode { get; set; }

        public virtual ICollection<Approval> Approvals { get; set; }
        [NotMapped]
        public string? ApproverComments { get; set; }
    }
}
