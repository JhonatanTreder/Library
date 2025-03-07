﻿using System.ComponentModel.DataAnnotations;

namespace API.DTO.Loan
{
    public class LoanFilterDTO
    {
        public int? Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O ID do usuário deve ser maior que 0.")]
        public int? UserId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O ID do livro deve ser maior que 0.")]
        public int? BookId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O ID do bibliotecário deve ser maior que 0.")]
        public int? LibrarianId { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Formato inválido para a data de empréstimo.")]
        public DateTime? LoanDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Formato inválido para a data de devolução.")]
        public DateTime? ReturnDate { get; set; }
    }

}
