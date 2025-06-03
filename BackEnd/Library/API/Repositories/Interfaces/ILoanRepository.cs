using API.DTOs.Loan;
using API.DTOs.Responses;
using API.Enum.Responses;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        Task<RepositoryResponse<Loan>> GetLoanByIdAsync(int id);
        Task<RepositoryResponse<IEnumerable<Loan>>> GetLoansAsync(LoanFilterDTO loanFilterDTO);
        Task<RepositoryStatus> UpdateLoanAsync(int id, LoanUpdateDTO loanUpdateDTOO);
        Task<RepositoryResponse<Loan>> AddLoanAsync(CreateLoanDTO loan);
        Task<RepositoryStatus> DeleteLoanAsync(int id);
        Task<RepositoryStatus> IsBookAvailableAsync(int id);
        Task<RepositoryStatus> RegisterReturnAsync(int id);
        Task<RepositoryStatus> ExtendLoanAsync(int loanId, DateTime newDate);
    }
}
