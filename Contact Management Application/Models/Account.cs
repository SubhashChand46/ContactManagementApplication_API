using System.ComponentModel.DataAnnotations;

namespace Contact_Management_Application.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
