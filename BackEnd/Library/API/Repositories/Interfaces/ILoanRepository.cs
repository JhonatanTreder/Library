using API.DTO.Loan;
using API.Enum.Responses;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        Task<Loan?> GetLoanByIdAsync(int id);
        Task<IEnumerable<Loan?>> GetLoansAsync(LoanFilterDTO loanFilterDTO);
        Task<LoanResponse> UpdateLoanAsync(int id, LoanUpdateDTO loanUpdateDTOO);
        Task<Loan?> AddLoanAsync(CreateLoanDTO loan);
        Task<LoanResponse> DeleteLoanAsync(int id);
        Task<LoanResponse> IsBookAvailableAsync(int id);
        Task<LoanResponse> RegisterReturnAsync(int id);
        Task<LoanResponse> ExtendLoanAsync(int loanId, DateTime newDate);
    }
}
