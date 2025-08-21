using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Indium.Common.DataTransferObjects
{
    public class RegularEmailRequest
    {
        public RegularEmailRequest()
        {
            CcRecipients = "";
            BccRecipients = "";
            AttachmentURL = "";
            IsBodyHtml = false; //HACK HACK, emails should eventually switch to HTML
        }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public string Recipients { get; set; }
        public string CcRecipients { get; set; }
        public string BccRecipients { get; set; }
        public string AttachmentURL { get; set; }
        public bool IsBodyHtml { get; set; }
    }
}


