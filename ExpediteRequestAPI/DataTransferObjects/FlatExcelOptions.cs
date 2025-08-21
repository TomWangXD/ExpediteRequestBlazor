using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Indium.Common.DataTransferObjects
{
    public class FlatExcelOptions<T>
    {
        public List<T> Items { get; set; }
        public string SheetName { get; set; }
        public string SheetPassword { get; set; }
        public string WorkbookPassword { get; set; }
        public List<Tuple<string, string, string>> Headers { get; set; } = new List<Tuple<string, string, string>>();
        public void AddHeader(string column, string label, string dataType = "string")
        {
            Headers.Add(new Tuple<string, string, string>(column, label, dataType));
        }
    }
}
