namespace API.Pagination
{
    public class PaginationParameters
    {
        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        public int _pageSize = 10;
        public int PageSize 
        {
            get
            {
                return _pageSize;
            }

            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
