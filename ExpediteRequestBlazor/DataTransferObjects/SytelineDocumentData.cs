using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpediteRequestBlazor.DataTransferObjects
{
    public class SytelineDocumentData
    {
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
    }
}
