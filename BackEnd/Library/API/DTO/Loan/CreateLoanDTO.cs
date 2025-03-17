using System.ComponentModel.DataAnnotations;

namespace API.DTO.Loan
{
    public class CreateLoanDTO
    {
        [Required(ErrorMessage = "O ID do usuário é obrigatório")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "O ID do livro é obrigatório")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "O ID do bibliotecário é obrigatório")]
        public int LibrarianId { get; set; }

        [Required(ErrorMessage = "A data de empréstimo é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "A data de empréstimo deve estar no formato válido de data")]
        public DateTime LoanDate { get; set; }

        [Required(ErrorMessage = "A data de devolução é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "A data de devolução deve estar no formato válido de data")]
        public DateTime ReturnDate { get; set; }
    }
}
