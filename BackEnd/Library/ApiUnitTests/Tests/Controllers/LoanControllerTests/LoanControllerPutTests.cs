using API.Controllers;
using API.DTO.Loan;
using API.DTO.Responses;
using API.Enum;
using API.Enum.Responses;
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
    public class LoanControllerPutTests
    {
        private readonly Mock<ILoanRepository> _loanRepositoryMock;
        private readonly LoanController _controller;

        public LoanControllerPutTests(LoanControllerFixture fixture)
        {
            _loanRepositoryMock = fixture.LoanRepositoryMock;
            _controller = fixture.Controller;
        }

        [Fact]
        public async Task PutLoan_ReturnNoContent()
        {
            int validId = 1;

            var newLoanInfo = new LoanUpdateDTO
            {
                ReturnDate = DateTime.Now,
                Status = LoanStatus.InProgress
            };

            _loanRepositoryMock.Setup(service => service.UpdateLoanAsync(validId, newLoanInfo))
                .ReturnsAsync(RepositoryStatus.Success);

            var putResult = await _controller.Put(validId, newLoanInfo);
            Assert.IsType<NoContentResult>(putResult);
        }

        [Fact]
        public async Task PutLoan_ReturnBadRequest_WhenLoanIsNull()
        {
            int loanId = 1;

            var newLoanInfo = new LoanUpdateDTO();

            _loanRepositoryMock.Setup(service => service.UpdateLoanAsync(loanId, newLoanInfo))
                .ReturnsAsync(RepositoryStatus.NullObject);

            var putResult = await _controller.Put(loanId, newLoanInfo);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O empréstimo não pode ser nulo", response.Message);
        }

        [Fact]
        public async Task PutLoan_ReturnNotFound_WhenLoanNotFound()
        {
            int loanId = 1;

            var newLoanInfo = new LoanUpdateDTO();

            _loanRepositoryMock.Setup(service => service.UpdateLoanAsync(loanId, newLoanInfo))
                .ReturnsAsync(RepositoryStatus.NotFound);

            var putResult = await _controller.Put(loanId, newLoanInfo);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O empréstimo de id '{loanId}' não foi encontrado", response.Message);
        }

        [Fact]
        public async Task PutLoan_ReturnNotFound_WhenBookNotFound()
        {
            int loanId = 1;

            var newLoanInfo = new LoanUpdateDTO();

            _loanRepositoryMock.Setup(service => service.UpdateLoanAsync(loanId, newLoanInfo))
                .ReturnsAsync(RepositoryStatus.BookNotFound);

            var putResult = await _controller.Put(loanId, newLoanInfo);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O livro que corresponde ao empréstimo de id '{loanId}' não foi encontrado", response.Message);
        }

        [Fact]
        public async Task PutLoan_ReturnConflict_WhenStatusTransitionIsInvalid()
        {
            int loanId = 1;

            var newLoanInfo = new LoanUpdateDTO();

            _loanRepositoryMock.Setup(service => service.UpdateLoanAsync(loanId, newLoanInfo))
                .ReturnsAsync(RepositoryStatus.InvalidStatusTransition);

            var putResult = await _controller.Put(loanId, newLoanInfo);
            var conflictResult = Assert.IsType<ConflictObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(conflictResult.Value);

            Assert.Equal("Conflict", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O novo status fornecido é inválido", response.Message);
        }

        [Fact]
        public async Task PutLoan_ReturnConflict_WhenReturnDateIsInvalid()
        {
            int loanId = 1;

            var newLoanInfo = new LoanUpdateDTO();

            _loanRepositoryMock.Setup(service => service.UpdateLoanAsync(loanId, newLoanInfo))
                .ReturnsAsync(RepositoryStatus.InvalidReturnDate);

            var putResult = await _controller.Put(loanId, newLoanInfo);
            var conflictResult = Assert.IsType<ConflictObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(conflictResult.Value);

            Assert.Equal("Conflict", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("A nova data fornecida é inválida", response.Message);
        }

        [Fact]
        public async Task ExtendLoan_ReturnNoContent()
        {
            int validId = 1;

            var newLoanDate = DateTime.UtcNow.AddDays(2);

            _loanRepositoryMock.Setup(service => service.ExtendLoanAsync(validId, newLoanDate))
                .ReturnsAsync(RepositoryStatus.Success);

            var putResult = await _controller.Put(validId, newLoanDate);
            Assert.IsType<NoContentResult>(putResult);
        }

        [Fact]
        public async Task ExtendLoan_ReturnBadRequest_WhenNewDateIsInvalid()
        {
            int loanId = 1;

            var newLoanDate = DateTime.UtcNow;

            _loanRepositoryMock.Setup(service => service.ExtendLoanAsync(loanId, newLoanDate))
                .ReturnsAsync(RepositoryStatus.InvalidDate);

            var putResult = await _controller.Put(loanId, newLoanDate);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("A nova data de devolução não pode ser menor ou igual a data de devolução antiga", response.Message);
        }

        [Fact]
        public async Task ExtendLoan_ReturnNotFound_WhenLoanNotFound()
        {
            int loanId = 1;

            var newLoanDate = DateTime.UtcNow;

            _loanRepositoryMock.Setup(service => service.ExtendLoanAsync(loanId, newLoanDate))
                .ReturnsAsync(RepositoryStatus.NotFound);

            var putResult = await _controller.Put(loanId, newLoanDate);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O empréstimo de id '{loanId}' não foi encontrado", response.Message);
        }

        [Fact]
        public async Task ExtendLoan_ReturnConflict_WhenStatusIsInvalid()
        {
            int loanId = 1;

            var newLoanDate = DateTime.UtcNow;

            _loanRepositoryMock.Setup(service => service.ExtendLoanAsync(loanId, newLoanDate))
                .ReturnsAsync(RepositoryStatus.InvalidStatus);

            var putResult = await _controller.Put(loanId, newLoanDate);
            var conflictResult = Assert.IsType<ConflictObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(conflictResult.Value);

            Assert.Equal("Conflict", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O status do empréstimo deve estar em 'in progress' para poder extender o prazo de devolucao", response.Message);
        }

        [Fact]
        public async Task RegisterReturn_ReturnNoContent()
        {
            int validId = 1;

            _loanRepositoryMock.Setup(service => service.RegisterReturnAsync(validId))
                .ReturnsAsync(RepositoryStatus.Success);

            var putResult = await _controller.Put(validId);
            Assert.IsType<NoContentResult>(putResult);
        }

        [Fact]
        public async Task RegisterReturn_ReturnNotFound_WhenLoanNotFound()
        {
            var loanId = 1;

            _loanRepositoryMock.Setup(service => service.RegisterReturnAsync(loanId))
                .ReturnsAsync(RepositoryStatus.NotFound);

            var putResult = await _controller.Put(loanId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O empréstimo de id '{loanId}' não foi encontrado", response.Message);
        }

        [Fact]
        public async Task RegisterReturn_ReturnConflict_WhenLoanStatusIsInvalid()
        {
            var loanId = 1;

            _loanRepositoryMock.Setup(service => service.RegisterReturnAsync(loanId))
                .ReturnsAsync(RepositoryStatus.InvalidStatus);

            var putResult = await _controller.Put(loanId);
            var conflictResult = Assert.IsType<ConflictObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(conflictResult.Value);

            Assert.Equal("Conflict", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O status do empréstimo deve estar em 'in progress' para ser registrado como 'finished'", response.Message);
        }
    }
}
