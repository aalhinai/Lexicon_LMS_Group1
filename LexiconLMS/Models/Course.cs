using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        [Display(Name = "Name")]
        public string CourseName { get; set; }

        [Display(Name = "Start Date")]
        public DateTime CourseStartDate { get; set; }

        [Display(Name = "End Date")]
        [GreaterThan("CourseStartDate", ErrorMessage = "End Date can't be before Start Date")]
        public DateTime CourseEndDate { get; set; }

        [Display(Name = "Description")]
        public string CourseDescription { get; set; }

        public virtual ICollection<ApplicationUser> Students { get; set; }
        public virtual ICollection<Module> Modules { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
    }
}