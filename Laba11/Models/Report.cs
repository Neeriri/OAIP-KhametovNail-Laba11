using System;

namespace Laba11.Models
{
    public class Report
    {
        public int Id { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Topic { get; set; }
        public int Grade { get; set; }
        public int StudentId { get; set; }
        public int? CompanyId { get; set; }
        public Student Student { get; set; }
        public Company Company { get; set; }
    }
}
