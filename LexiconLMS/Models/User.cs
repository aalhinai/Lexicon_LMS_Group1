using System;
using System.Collections.Generic;

namespace LexiconLMS.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserFullName { get { return UserFirstName + UserLastName; } }
        public string UserEmail { get; set; }
        public DateTime UserStartDate { get; set; }
        public int? CourseId { get; set; }

        public virtual Course Course { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
    }
}