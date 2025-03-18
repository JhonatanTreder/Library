using API.Context;
using API.DTO.Loan;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly AppDbContext _context;
        public LoanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Loan?> AddLoanAsync(CreateLoanDTO createLoanDTO)
        {
            if (createLoanDTO is null)
            {
                return null;
            }

            var loan = new Loan
            {
                UserId = createLoanDTO.UserId,
                BookId = createLoanDTO.BookId,
                LibrarianId = createLoanDTO.LibrarianId,
                LoanDate = createLoanDTO.LoanDate,
                ReturnDate = createLoanDTO.ReturnDate,
                Status = createLoanDTO.Status
            };

            await _context.AddAsync(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task<bool> DeleteLoanAsync(int id)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan is null)
            {
                return false;
            }

            _context.Remove(loan);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Loan?> GetLoanByIdAsync(int id)
        {
            return await _context.Loans.FindAsync();
        }

        public async Task<IEnumerable<Loan?>> GetLoansAsync(LoanFilterDTO loanFilterDTO)
        {
            var query = _context.Loans.AsQueryable();

            if (loanFilterDTO.Id.HasValue)
                query = query.Where(i => i.Id == loanFilterDTO.Id);

            if (loanFilterDTO.UserId.HasValue)
                query = query.Where(i => i.UserId == loanFilterDTO.UserId);

            if (loanFilterDTO.LibrarianId.HasValue)
                query = query.Where(i => i.LibrarianId == loanFilterDTO.LibrarianId);

            if (loanFilterDTO.LoanDate.HasValue)
                query = query.Where(l => l.LoanDate == loanFilterDTO.LoanDate);

            if (loanFilterDTO.LoanDate.HasValue)
                query = query.Where(l => l.ReturnDate == loanFilterDTO.ReturnDate);

            return await query.ToListAsync();
        }

        public async Task<bool> UpdateLoanAsync(int id, LoanUpdateDTO loanUpdateDTO)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan is null)
            {
                return false;
            }

            loan.ReturnDate = loanUpdateDTO.ReturnDate;
            loan.Status = loanUpdateDTO.Status;

            _context.Update(loan);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
