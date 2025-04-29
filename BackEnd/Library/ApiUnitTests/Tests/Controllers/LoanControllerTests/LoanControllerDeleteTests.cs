using API.Controllers;
using API.DTO.Responses;
using API.Enum.Responses;
using API.Repositories.Interfaces;
using ApiUnitTests.Fixtures;
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
    public class LoanControllerDeleteTests
    {
        private readonly Mock<ILoanRepository> _loanRepositoryMock;
        private readonly LoanController _controller;

        public LoanControllerDeleteTests(LoanControllerFixture fixture)
        {
            _loanRepositoryMock = fixture.LoanRepositoryMock;
            _controller = fixture.Controller;
        }

        [Fact]
        public async Task DeleteLoan_ReturnNoContent()
        {
            int validLoanId = 1;

            _loanRepositoryMock.Setup(service => service.DeleteLoanAsync(validLoanId))
                .ReturnsAsync(RepositoryStatus.Success);

            var deleteResult = await _controller.Delete(validLoanId);
            Assert.IsType<NoContentResult>(deleteResult);
        }

        [Fact]
        public async Task DeleteLoan_ReturnNotFound_WhenLoanNotFound()
        {
            var loanId = 1;

            _loanRepositoryMock.Setup(service => service.DeleteLoanAsync(loanId))
                .ReturnsAsync(RepositoryStatus.NotFound);

            var deleteResult = await _controller.Delete(loanId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(deleteResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O empréstimo de id '{loanId}' não foi encontrado", response.Message);
        }

        [Fact]
        public async Task DeleteLoan_ReturnConflict_WhenLoanCannotDelete()
        {
            var loanId = 1;

            _loanRepositoryMock.Setup(service => service.DeleteLoanAsync(loanId))
                .ReturnsAsync(RepositoryStatus.CannotDelete);

            var deleteResult = await _controller.Delete(loanId);
            var conflictResult = Assert.IsType<ConflictObjectResult>(deleteResult);
            var response = Assert.IsType<ApiResponse>(conflictResult.Value);

            Assert.Equal("Conflict", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Não é possível deletar um empréstimo com o status 'pending' ou 'in progress'", response.Message);
        }
    }
}
