using System;

namespace AttributeRoutingSample.Models
{
    public class Contact
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime LastModified { get; set; }
    }
}
