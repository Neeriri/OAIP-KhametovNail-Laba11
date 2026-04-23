using System.Collections.Generic;

namespace Laba11.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Industry { get; set; }
        public string City { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
