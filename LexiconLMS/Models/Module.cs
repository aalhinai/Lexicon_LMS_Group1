using System;
using System.Collections.Generic;

namespace LexiconLMS.Models
{
    public class Module
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleDescription { get; set; }
        public DateTime ModuleStartDate { get; set; }
        public DateTime ModuleEndDate { get; set; }
        public int CourseId { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
    }
}