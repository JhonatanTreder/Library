using API.Enum;
using System.ComponentModel.DataAnnotations;
namespace API.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public UserType Type { get; set; }
        public ICollection<Loan>? Loans { get; set; }
    }
}
