using System;
using System.ComponentModel.DataAnnotations;

namespace Contacts.API.Models
{
    public class Contact
    {
        public int ContactID { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(100)]
        public string Email { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
    }
}
