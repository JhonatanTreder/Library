﻿using API.Context;
using API.DTOs.Loan;
using API.DTOs.Responses;
using API.Enum;
using API.Enum.Responses;
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

        public async Task<RepositoryResponse<Loan>> AddLoanAsync(CreateLoanDTO createLoanDTO)
        {
            if (createLoanDTO is null)
            {
                return new RepositoryResponse<Loan>(RepositoryStatus.NullObject);
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

            return new RepositoryResponse<Loan>(RepositoryStatus.Success, loan);
        }

        public async Task<RepositoryStatus> DeleteLoanAsync(int id)
        {
            var loan = await GetLoanByIdAsync(id);

            if (loan.Data is null)
                return RepositoryStatus.NotFound;

            if (loan.Data.Status == LoanStatus.InProgress ||
                loan.Data.Status == LoanStatus.Pending)
                return RepositoryStatus.CannotDelete;

            _context.Remove(loan);
            await _context.SaveChangesAsync();

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryResponse<Loan>> GetLoanByIdAsync(int id)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan is not null)
            { 
                return new RepositoryResponse<Loan>(RepositoryStatus.Success, loan);
            }

            else
            {  
                return new RepositoryResponse<Loan>(RepositoryStatus.NotFound);
            }
        }

        public async Task<RepositoryStatus> IsBookAvailableAsync(int id)
        {
            var bookAvailable = !await _context.Loans.AnyAsync(l => l.BookId == id && l.Status == LoanStatus.InProgress);

            if (bookAvailable is false)
                return RepositoryStatus.BookNotAvailable;

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryStatus> RegisterReturnAsync(int id)
        {
            var loan = await GetLoanByIdAsync(id);

            if (loan.Data is null)
                return RepositoryStatus.NotFound;

            if (loan.Data.Status != LoanStatus.InProgress)
                return RepositoryStatus.InvalidStatus;

            loan.Data.Status = LoanStatus.Finished;
            loan.Data.ReturnDate = DateTime.UtcNow;

            _context.Update(loan);
            await _context.SaveChangesAsync();

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryStatus> ExtendLoanAsync(int id, DateTime newDate)
        {
            var loan = await GetLoanByIdAsync(id);

            if (loan.Data is null)
                return RepositoryStatus.NotFound;

            if (loan.Data.Status != LoanStatus.InProgress)
                return RepositoryStatus.InvalidStatus;

            if (newDate <= loan.Data.ReturnDate)
                return RepositoryStatus.InvalidDate;

            loan.Data.ReturnDate = newDate;
            _context.Loans.Update(loan.Data);

            await _context.SaveChangesAsync();

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryResponse<IEnumerable<Loan>>> GetLoansAsync(LoanFilterDTO loanFilterDTO)
        {
            if (loanFilterDTO is null)
            {
                return new RepositoryResponse<IEnumerable<Loan>>(RepositoryStatus.NullObject);
            }

            var query = _context.Loans.AsQueryable();

            if (loanFilterDTO.Id.HasValue)
                query = query.Where(i => i.Id == loanFilterDTO.Id);

            if (!string.IsNullOrWhiteSpace(loanFilterDTO.UserId))
                query = query.Where(i => i.UserId == loanFilterDTO.UserId);

            if (loanFilterDTO.BookId.HasValue)
                query = query.Where(i => i.BookId == loanFilterDTO.BookId);

            if (!string.IsNullOrWhiteSpace(loanFilterDTO.LibrarianId))
                query = query.Where(i => i.LibrarianId == loanFilterDTO.LibrarianId);

            if (loanFilterDTO.LoanDate.HasValue)
                query = query.Where(l => l.LoanDate == loanFilterDTO.LoanDate);

            if (loanFilterDTO.ReturnDate.HasValue)
                query = query.Where(l => l.ReturnDate == loanFilterDTO.ReturnDate);

            var loans = await query.ToListAsync();

            if (loans.Count > 0)
            {
                return new RepositoryResponse<IEnumerable<Loan>>(RepositoryStatus.Success, loans);
            }

            else
            {
                return new RepositoryResponse<IEnumerable<Loan>>(RepositoryStatus.NotFound);
            }
        }

        public async Task<RepositoryStatus> UpdateLoanAsync(int id, LoanUpdateDTO loanUpdateDTO)
        {
            if (loanUpdateDTO is null)
                return RepositoryStatus.NullObject;

            var loan = await GetLoanByIdAsync(id);

            if (loan.Data is null)
                return RepositoryStatus.NotFound;

            if (loan.Data.Status == LoanStatus.Finished && loanUpdateDTO.Status != LoanStatus.Canceled)
                return RepositoryStatus.InvalidStatusTransition;

            if (loan.Data.Status == LoanStatus.Canceled && loanUpdateDTO.Status != LoanStatus.Finished)
                return RepositoryStatus.InvalidStatusTransition;

            if (loanUpdateDTO.ReturnDate < DateTime.UtcNow.Date && loanUpdateDTO.Status != LoanStatus.Finished)
                return RepositoryStatus.InvalidReturnDate;

            loan.Data.ReturnDate = loanUpdateDTO.ReturnDate;
            loan.Data.Status = loanUpdateDTO.Status;

            if (loan.Data.Status == LoanStatus.Finished)
            {
                var bookCopy = await _context.BookCopies
                                             .FirstOrDefaultAsync(bc => bc.BookId == loan.Data.BookId 
                                             && bc.Loans != null 
                                             && bc.Loans
                                             .Any(l => l.Id == loan.Data.Id));

                if (bookCopy is null)
                    return RepositoryStatus.BookNotFound;

                bookCopy.Status = BookStatus.Available;

                _context.Update(bookCopy);
            }

            _context.Update(loan);
            await _context.SaveChangesAsync();

            return RepositoryStatus.Success;
        }
    }
}
