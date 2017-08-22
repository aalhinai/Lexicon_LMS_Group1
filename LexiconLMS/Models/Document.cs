using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Models
{
    public class Document
    {
        [DefaultValue(NotCompleted)]
        public enum StatusType
        {
            NotCompleted,
            Completed,
            Redo,


        }

        [Key]
        public int DocId { get; set; }
        public string DocName { get; set; }
        [Display(Name = "Description")]
        public string DocDescription { get; set; }
        public DateTime DocTimestamp { get; set; }
        public DateTime? DocDeadline { get; set; }
        public string UserId { get; set; }
        public int? CourseId { get; set; }
        public int? ModuleId { get; set; }
        public int? ActivityId { get; set; }
        public string DocURL { get; set; }
        [Display(Name = " FeedBack")]
        public string FeedBack { get; set; }
        [Display(Name = "Status")]
        public StatusType Status {  get; set; } 

        public virtual ApplicationUser User { get; set; }
        public virtual Course Course { get; set; }
        public virtual Module Module { get; set; }
        public virtual Activity Activity { get; set; }
    }
}