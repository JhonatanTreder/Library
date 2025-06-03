using API.Controllers;
using API.DTOs.Loan;
using API.DTOs.Responses;
using API.Enum.Responses;
using API.Models;
using API.Repositories.Interfaces;
using ApiUnitTests.Fixtures.Controllers;
using FluentAssertions;
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
    public class LoanControllerPostTests
    {
        private readonly Mock<ILoanRepository> _loanRepositoryMock;
        private readonly LoanController _controller;

        public LoanControllerPostTests(LoanControllerFixture fixture)
        {
            _loanRepositoryMock = fixture.LoanRepositoryMock;
            _controller = fixture.Controller;
        }

        [Fact]
        public async Task PostLoan_ReturnCreated()
        {
            var loanInfo = new CreateLoanDTO
            {
                BookId = 1,
                UserId = "V4L1D-ID",
                LibrarianId = "V4L1D-LIBR4RIAN-ID",
                LoanDate = DateTime.UtcNow,
                ReturnDate = DateTime.UtcNow.AddDays(7)
            };

            var loan = new Loan
            {
                Id = 1,
                BookId = loanInfo.BookId,
                UserId = loanInfo.UserId,
                LibrarianId = loanInfo.LibrarianId,
                LoanDate = loanInfo.LoanDate,
                ReturnDate = loanInfo.ReturnDate
            };

            _loanRepositoryMock.Setup(service => service.AddLoanAsync(loanInfo))
                .ReturnsAsync(new RepositoryResponse<Loan>(RepositoryStatus.Success, loan));

            var postResult = await _controller.Post(loanInfo);
            var createdResult = Assert.IsType<CreatedAtActionResult>(postResult);
            var response = Assert.IsType<ApiResponse>(createdResult.Value);

            Assert.Equal("Created", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Empréstimo criado com sucesso", response.Message);
        }

        [Fact]
        public async Task PostLoan_ReturnBadRequest_WhenLoanInfoIsNull()
        {
            var loanInfo = new CreateLoanDTO();

            _loanRepositoryMock.Setup(service => service.AddLoanAsync(loanInfo))
                .ReturnsAsync(new RepositoryResponse<Loan>(RepositoryStatus.NullObject));

            var postResult = await _controller.Post(loanInfo);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(postResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O novo empréstimo não pode ser nulo", response.Message);
        }
    }
}
