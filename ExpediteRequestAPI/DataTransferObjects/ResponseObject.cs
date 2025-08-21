using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Indium.Common.DataTransferObjects
{
    public class ResponseObject<T>
    {
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public T Payload {get;set;}
    }
}
