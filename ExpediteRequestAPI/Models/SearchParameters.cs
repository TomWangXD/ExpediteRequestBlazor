using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Indium.Common.Models
{
    public class SearchParameters
    {
        public int Page { get; set; }
        public int Size { get; set; }

#nullable enable
        public string? SortBy { get; set; }
        public string? Direction { get; set; }

        /* Search Parameters */
        
        public Guid? Gkey { get; set; }
        public string? CoNum { get; set; }
        public short? CoLine { get; set; }
        public short? CoRelease { get; set; }
        public string? PlanCode { get; set; }
        public string? ShipSite { get; set; }
        public string? Job { get; set; }
        public string? Ipn { get; set; }
        public string? IpnDescription { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? CustomerName { get; set; }
        public int? QtyOrdered { get; set; }
        public string? Um { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? ShelfLifeRequirementFlag { get; set; }
        public string? ShelfLifeRequirement { get; set; }
        public bool? PonderosaPack { get; set; }
        public bool? CurrentDueDate { get; set; }
        public bool? PartialQuantityAccepted { get; set; }
        public int? PartialQuantity { get; set; }
        public DateTime? ShipDate { get; set; }
        public bool? Fee { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public string? Comments { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? Created { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? Modified { get; set; }

        /* END Search Parameters */
#nullable disable

        public SearchParameters()
        {
            Page = 1;
            Size = 20;
            SortBy = string.Empty;
            Direction = string.Empty;
        }

        public SearchParameters(int p, int s)
        {
            // pages & sizes can't be less than one, and sizes should be clamped to something reasonable
            Page = (p < 1) ? 1 : p;
            Size = (s > 50 || s < 1) ? 50 : p;
            SortBy = string.Empty;
            Direction = string.Empty;
        }
    }
}