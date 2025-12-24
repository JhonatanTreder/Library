namespace API.DTOs.Pagination
{
    public class PaginatedDataDTO<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public bool HasNext => CurrentPage < TotalPages;
        public bool HasPrevious => CurrentPage > 1;
        public IEnumerable<T>? Data { get; set; }
    }
}
