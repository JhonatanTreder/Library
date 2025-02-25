﻿using API.Enum;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID do usuário é obrigatório")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "O ID do livro é obrigatório")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "O ID do bibliotecário é obrigatório")]
        public int LibrarianId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade mínima de empréstimo é 1")]
        public int? LoanQuantity { get; set; }

        [Required(ErrorMessage = "A data de empréstimo é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "A data de empréstimo deve estar no formato válido de data")]
        public DateTime LoanDate { get; set; }

        [Required(ErrorMessage = "A data de devolução é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "A data de devolução deve estar no formato válido de data")]
        public DateTime ReturnDate { get; set; }

        [Required(ErrorMessage = "O status do empréstimo é obrigatório")]
        [EnumDataType(typeof(LoanStatus), ErrorMessage = "O status de empréstimo deve ser um valor válido")]
        public LoanStatus Status { get; set; }

        public bool IsValidDateRange()
        {
            return LoanDate >= DateTime.Today && ReturnDate > LoanDate;
        }
    }
}
