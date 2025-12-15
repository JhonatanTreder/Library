using API.Enum;

namespace API.DTOs.BookDTOs
{
    public class BookCopyReturnDTO
    {
        public int CopyId { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public DateTime AcquiredAt { get; set; }

        public string Status { get; set; } = string.Empty;

        private static string GetStatusDescription(string status)
        {
            return status switch
            {
                "Available" => "Disponível",
                "Borrowed" => "Emprestado",
                "Reserved" => "Reservado",
                "UnderMaintenance" => "Em Manutenção",
                "Lost" => "Perdido",
                "Damaged" => "Danificado",
                "Archived" => "Arquivado",
                _ => status
            };
        }
    }
}
