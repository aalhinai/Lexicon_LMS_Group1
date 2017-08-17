using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Models
{
    public class Module
    {
        public int ModuleId { get; set; }

        [Display(Name = "Name")]
        public string ModuleName { get; set; }

        [Display(Name = "Description")]
        public string ModuleDescription { get; set; }

        [Display(Name = "Start Date")]
        public DateTime ModuleStartDate { get; set; }

        [Display(Name = "End Date")]
        [GreaterThan("ModuleStartDate", ErrorMessage = "End Date can't be before Start Date")]
        public DateTime ModuleEndDate { get; set; }

        public int CourseId { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
    }
}