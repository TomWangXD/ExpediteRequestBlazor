using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Indium.Common.DataTransferObjects
{
    public class SearchResults<T>
    {
        public List<T> Results { get; set; }
        public int CurrentPage { get; set; }
        public int TotalResults { get; set; }
        public int CurrentPageResults { get { return Results.Count; } }
        public int TotalPages { get; set; }
        public string Error { get; set; }
    }
}
