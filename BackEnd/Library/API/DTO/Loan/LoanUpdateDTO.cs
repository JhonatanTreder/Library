using API.Enum;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace API.DTO.Loan
{
    public class LoanUpdateDTO
    {
        [Required(ErrorMessage = "A data de devolução é obrigatória.")]
        [DataType(DataType.Date, ErrorMessage = "A data deve estar em um formato válido de data.")]
        public DateTime ReturnDate { get; set; }

        [Required(ErrorMessage = "O status do empréstimo é obrigatório.")]
        [EnumDataType(typeof(LoanStatus), ErrorMessage = "O status deve estar em um formato válido.")]
        public LoanStatus Status { get; set; }
    }
}
