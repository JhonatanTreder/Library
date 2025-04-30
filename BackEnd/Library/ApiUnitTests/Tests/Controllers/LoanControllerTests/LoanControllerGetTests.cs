using API.Controllers;
using API.DTO.Loan;
using API.DTO.Responses;
using API.Enum.Responses;
using API.Models;
using API.Repositories.Interfaces;
using ApiUnitTests.Fixtures.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Tests.Controllers.LoanControllerTests
{
    [Collection("LoanControllerCollection")]
    public class LoanControllerGetTests
    {
        private readonly Mock<ILoanRepository> _loanRepositoryMock;
        private readonly LoanController _controller;

        public LoanControllerGetTests(LoanControllerFixture fixture)
        {
            _loanRepositoryMock = fixture.LoanRepositoryMock;
            _controller = fixture.Controller;
        }

        [Fact]
        public async Task GetLoans_ReturnOk()
        {
            var loanFilter = new LoanFilterDTO
            {
                Id = 1,
                BookId = 1,
                UserId = "US3R-ID",
                LibrarianId = "LIBR4RIAN-ID",
                LoanDate = DateTime.UtcNow,
                ReturnDate = DateTime.UtcNow.AddDays(2)
            };

            var loans = new List<Loan> 
            {
                new Loan(),
                new Loan(),
                new Loan()
            };

            _loanRepositoryMock.Setup(service => service.GetLoansAsync(loanFilter))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<Loan>>(RepositoryStatus.Success, loans));

            var getResult = await _controller.Get(loanFilter);
            var okSuccessResult = Assert.IsType<OkObjectResult>(getResult);
            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);

            Assert.Equal("Ok", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Empréstimos encontrados com sucesso", response.Message);
        }

        [Fact]
        public async Task GetLoans_ReturnBadrequest_WhenLoanIsNull()
        {
            var loanFilter = new LoanFilterDTO();

            _loanRepositoryMock.Setup(service => service.GetLoansAsync(loanFilter))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<Loan>>(RepositoryStatus.NullObject));

            var getResult = await _controller.Get(loanFilter);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(getResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O empréstimo não pode ser nulo", response.Message);
        }

        [Fact]
        public async Task GetLoans_ReturnNotFound_WhenLoansNotFound()
        {
            var loanFilter = new LoanFilterDTO
            {
                Id = 1,
                BookId = 1,
                UserId = "US3R-ID",
                LibrarianId = "LIBR4RIAN-ID",
                LoanDate = DateTime.UtcNow,
                ReturnDate = DateTime.UtcNow.AddDays(2)
            };

            _loanRepositoryMock.Setup(service => service.GetLoansAsync(loanFilter))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<Loan>>(RepositoryStatus.NotFound));

            var getResult = await _controller.Get(loanFilter);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(getResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Nenhum empréstimo foi encontrado", response.Message);
        }
    }
}
