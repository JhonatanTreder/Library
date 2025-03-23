using API.DTO.Loan;
using API.Enum.Responses.Loan;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        Task<Loan?> AddLoanAsync(CreateLoanDTO loan);
        Task<IEnumerable<Loan?>> GetLoansAsync(LoanFilterDTO loanFilterDTO);
        Task<Loan?> GetLoanByIdAsync(int id);
        Task<LoanResponse> IsBookAvailableAsync(int id);
        Task<LoanResponse> RegisterReturnAsync(int loanId);
        Task<LoanResponse> ExtendLoanAsync(int loanId, DateTime newDate);
        Task<LoanResponse> UpdateLoanAsync(int id, LoanUpdateDTO loanUpdateDTOO);
        Task<LoanResponse> DeleteLoanAsync(int id);
    }
}
