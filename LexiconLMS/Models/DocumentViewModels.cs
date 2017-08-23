using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LexiconLMS.Models
{
    public class FeedbackViewModel
    {
        public int DocId { get; set; }
        public string FeedBack { get; set; }
        public StatusType Status { get; set; }
    }
}