using System;
using System.Collections.Generic;

namespace LexiconLMS.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime CourseStartDate { get; set; }
        public DateTime CourseEndDate { get; set; }
        public string CourseDescription { get; set; }

        public virtual ICollection<User> Students { get; set; }
        public virtual ICollection<Module> Modules { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
    }
}