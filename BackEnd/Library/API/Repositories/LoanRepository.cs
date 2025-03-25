using API.Context;
using API.DTO.Loan;
using API.Enum;
using API.Enum.Responses;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

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
                Status = LoanStatus.Pending
            };

            await _context.AddAsync(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task<LoanResponse> DeleteLoanAsync(int id)
        {
            var loan = await GetLoanByIdAsync(id);

            if (loan is null)
                return LoanResponse.NotFound;

            if (loan.Status == LoanStatus.InProgress ||
                loan.Status == LoanStatus.Pending)
                return LoanResponse.CannotDelete;

            _context.Remove(loan);
            await _context.SaveChangesAsync();

            return LoanResponse.Success;
        }

        public async Task<Loan?> GetLoanByIdAsync(int id)
        {
            return await _context.Loans.FindAsync(id);
        }

        public async Task<LoanResponse> IsBookAvailableAsync(int id)
        {
            var bookAvailable =  !await _context.Loans.AnyAsync(l => l.BookId == id && l.Status == LoanStatus.InProgress);

            if (bookAvailable is false)
                return LoanResponse.BookNotAvailable;

            return LoanResponse.Success;
        }

        public async Task<LoanResponse> RegisterReturnAsync(int id)
        {
            var loan = await GetLoanByIdAsync(id);

            if (loan is null)
                return LoanResponse.NotFound;

            if (loan.Status != LoanStatus.InProgress)
                return LoanResponse.InvalidStatus;

            loan.Status = LoanStatus.Finished;
            loan.ReturnDate = DateTime.UtcNow;

            _context.Update(loan);
            await _context.SaveChangesAsync();

            return LoanResponse.Success;
        }

        public async Task<LoanResponse> ExtendLoanAsync(int id, DateTime newDate)
        {
            var loan = await GetLoanByIdAsync(id);

            if (loan is null)
                return LoanResponse.NotFound;

            if (loan.Status != LoanStatus.InProgress)
                return LoanResponse.InvalidStatus;

            if (newDate > loan.ReturnDate)
                return LoanResponse.InvalidDate;

            loan.ReturnDate = newDate;
            _context.Loans.Update(loan);

            await _context.SaveChangesAsync();

            return LoanResponse.Success;
        }

        public async Task<IEnumerable<Loan?>> GetLoansAsync(LoanFilterDTO loanFilterDTO)
        {
            var query = _context.Loans.AsQueryable();

            if (loanFilterDTO.Id.HasValue)
                query = query.Where(i => i.Id == loanFilterDTO.Id);

            if (loanFilterDTO.UserId.HasValue)
                query = query.Where(i => i.UserId == loanFilterDTO.UserId);

            if (loanFilterDTO.BookId.HasValue)
                query = query.Where(i => i.BookId == loanFilterDTO.BookId);

            if (loanFilterDTO.LibrarianId.HasValue)
                query = query.Where(i => i.LibrarianId == loanFilterDTO.LibrarianId);

            if (loanFilterDTO.LoanDate.HasValue)
                query = query.Where(l => l.LoanDate == loanFilterDTO.LoanDate);

            if (loanFilterDTO.ReturnDate.HasValue)
                query = query.Where(l => l.ReturnDate == loanFilterDTO.ReturnDate);

            return await query.ToListAsync();
        }

        public async Task<LoanResponse> UpdateLoanAsync(int id, LoanUpdateDTO loanUpdateDTO)
        {
            var loan = await GetLoanByIdAsync(id);

            if (loanUpdateDTO is null)
                return LoanResponse.NullObject;

            if (loan is null)
                return LoanResponse.NotFound;

            if (loan.Status == LoanStatus.Finished && loanUpdateDTO.Status != LoanStatus.Canceled)
                return LoanResponse.InvalidStatusTransition;

            if (loan.Status == LoanStatus.Canceled && loanUpdateDTO.Status != LoanStatus.Finished)
                return LoanResponse.InvalidStatusTransition;

            if (loanUpdateDTO.ReturnDate < DateTime.UtcNow.Date && loanUpdateDTO.Status != LoanStatus.Finished)
                return LoanResponse.InvalidReturnDate;

            loan.ReturnDate = loanUpdateDTO.ReturnDate;
            loan.Status = loanUpdateDTO.Status;

            if (loan.Status == LoanStatus.Finished)
            {
                var book = await _context.Books.FindAsync(loan.BookId);

                if (book is null)
                {
                    return LoanResponse.BookNotFound;
                }

                book.Status = BookStatus.Available;
                _context.Update(book);
            }

            _context.Update(loan);
            await _context.SaveChangesAsync();

            return LoanResponse.Success;
        }
    }
}
