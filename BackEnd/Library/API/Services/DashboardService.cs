using API.DTOs.Dashboard;
using API.DTOs.Responses;
using API.Enum.Responses;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository _userRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IEventRepository _eventRepository;

        public DashboardService(
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IUserRepository userRepository, IBookRepository bookRepository,
            ILoanRepository loanRepository, IEventRepository eventRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _bookRepository = bookRepository;
            _loanRepository = loanRepository;
            _eventRepository = eventRepository;
        }

        public async Task<RepositoryResponse<LibraryStatsDTO>> GetLibraryStatsAsync()
        {
            var newBooks = await _bookRepository.GetRecentBooksAsync();
            var totalBooks = await _bookRepository.GetBooksAsync();
            var delayedBooks = await _bookRepository.GetBorrowedBooksAsync();
            var unavailableBooks = await _bookRepository.GetUnavailableBookCopiesAsync();
            var activeEvents = await _eventRepository.GetActiveEventsAsync();

            var libraryStats = new LibraryStatsDTO
            {
                RecentBooks = newBooks.Data,
                TotalBooks = totalBooks.Data,
                DelayedBooks = delayedBooks.Data,
                UnavailableBooks = unavailableBooks.Data,
                ActiveEvents = activeEvents.Data,
            };

            return new RepositoryResponse<LibraryStatsDTO>(RepositoryStatus.Success, libraryStats);
        }
    }
}
