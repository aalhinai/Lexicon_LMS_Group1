using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Display(Name = "Type")]
        public ActivityType ActivityType { get; set; }

        [Display(Name = "Name")]
        public string ActivityName { get; set; }

        [Display(Name = "Start Date")]
        public DateTime ActivityStartDate { get; set; }

        [Display(Name = "End Date")]
        [GreaterThan("ActivityStartDate", ErrorMessage = "End Date can't be before Start Date")]
        public DateTime ActivityEndDate { get; set; }

        [Display(Name = "Description")]
        public string ActivityDescription { get; set; }

        public int ModuleId { get; set; }

        public virtual Module Module { get; set; }
        public virtual ICollection<Document> Documents { get; set; }

    }
}