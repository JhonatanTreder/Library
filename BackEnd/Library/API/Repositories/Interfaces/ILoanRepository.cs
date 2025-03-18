using API.DTO.Loan;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        Task<Loan?> AddLoanAsync(CreateLoanDTO loan);
        Task<IEnumerable<Loan?>> GetLoansAsync(LoanFilterDTO loanFilterDTO);
        Task<Loan?> GetLoanByIdAsync(int id);
        Task<bool> UpdateLoanAsync(int id, LoanUpdateDTO loanUpdateDTOO);
        Task<bool> DeleteLoanAsync(int id);
    }
}
