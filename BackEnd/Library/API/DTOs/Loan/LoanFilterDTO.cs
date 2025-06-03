using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Loan
{
    public class LoanFilterDTO
    {
        public int? Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O ID do usuário deve ser maior que 0.")]
        public string? UserId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O ID do livro deve ser maior que 0.")]
        public int? BookId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O ID do bibliotecário deve ser maior que 0.")]
        public string? LibrarianId { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Formato inválido para a data de empréstimo.")]
        public DateTime? LoanDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Formato inválido para a data de devolução.")]
        public DateTime? ReturnDate { get; set; }
    }

}
