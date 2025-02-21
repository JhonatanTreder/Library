using API.Enum;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int LibrarianId { get; set; }
        public int LoanQuantity { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public LoanStatus Status { get; set; }
    }
}
