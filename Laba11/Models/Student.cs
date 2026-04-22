using System.Collections.Generic;

namespace Laba11.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string University { get; set; }
        public string Specialty { get; set; }

        // Навигационные свойства
        public Company Company { get; set; }
        public int? CompanyId { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}
