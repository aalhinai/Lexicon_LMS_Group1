﻿using System;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Models
{
    public class Document
    {
        [Key]
        public int DocId { get; set; }
        public string DocName { get; set; }
        public string DocDescription { get; set; }
        public DateTime DocTimestamp { get; set; }
        public DateTime? DocDeadline { get; set; }
        public int UserId { get; set; }
        public int? CourseId { get; set; }
        public int? ModuleId { get; set; }
        public int? ActivityId { get; set; }


        public virtual ApplicationUser User { get; set; }
        public virtual Course Course { get; set; }
        public virtual Module Module { get; set; }
        public virtual Activity Activity { get; set; }
    }
}