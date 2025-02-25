using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título do evento é obrigatório")]
        [StringLength(80, MinimumLength = 1, ErrorMessage = "O título do evento deve conter entre 1 a 80 caracteres")]
        public string Title { get; set; } = string.Empty;

        [StringLength(400, MinimumLength = 1, ErrorMessage = "A descrição do evento deve conter entre 1 a 400 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "O público alvo é obrigatório")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "O público alvo deve conter entre 5 a 20 caracteres")]
        public string TargetAudience { get; set; } = string.Empty;

        [Required(ErrorMessage = "A localização é obrigatória")]
        [StringLength(45, MinimumLength = 1, ErrorMessage = "O local do evento deve conter entre 1 a 45 caracteres")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de início do evento é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "A data de início do evento deve estar em um formato válido de data")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "A data de término do evento é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "A data de término do evento deve estar em um formato válido de data")]
        public DateTime EndDate { get; set; }

        public bool IsValidDateRange()
        {
            return EndDate >= DateTime.Today && EndDate >= StartDate;
        }
    }
}
