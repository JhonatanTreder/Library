using API.Enum.Responses;

namespace API.DTO.Responses
{
    public class RepositoryResponse<T>
    {
        public T? Data { get; set; }
        public RepositoryStatus Status { get; set; }

        public RepositoryResponse(RepositoryStatus status, T? data = default)
        {
            Status = status;
            Data = data;
        }
    }
}
