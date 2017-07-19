using System;
using System.Collections.Generic;

namespace LexiconLMS.Models
{
    public enum ActivityType
    {
        Assignment,
        ELearning,
        Excersice,
        Lecture

    }
    public class Activity
    {
        public int ActivityId { get; set; }
        public ActivityType ActivityType { get; set; }
        public string ActivityName { get; set; }
        public DateTime ActivityStartDate { get; set; }
        public DateTime ActivityEndDate { get; set; }
        public string ActivityDescription { get; set; }
        public int ModuleId { get; set; }

        public virtual Module Module { get; set; }
        public virtual ICollection<Document> Documents { get; set; }

    }
}