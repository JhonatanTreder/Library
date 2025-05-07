using API.Controllers;
using API.DTO.Book;
using API.DTO.Responses;
using API.DTOs.Book;
using API.Enum;
using API.Enum.Responses;
using API.Repositories.Interfaces;
using ApiUnitTests.Fixtures.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Tests.Controllers.BookControllerTests
{
    [Collection("BookControllerCollection")]
    public class BookControllerGetTests
    {
        private readonly BookController _controller;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        public BookControllerGetTests(BookControllerFixture fixture)
        {
            _controller = fixture.Controller;
            _bookRepositoryMock = fixture.BookRepositoryMock;
        }

        [Fact]
        public async Task GetBooks_ReturnOk()
        {
            var bookFilter = new BookFilterDTO
            {
                Title = "valid-title",
                Author = "valid-auhtor",
                Category = "valid-category",
                Publisher = "valid-publisher",
                PublicationYear = 2000,
            };

            var booksQuery = new List<BookReturnDTO>
            {
                new BookReturnDTO(),
                new BookReturnDTO(),
                new BookReturnDTO()
            };

            _bookRepositoryMock.Setup(service => service.GetBooksAsync(bookFilter))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.Success, booksQuery));

            var getBooksResult = await _controller.Get(bookFilter);
            var okSuccessResult = Assert.IsType<OkObjectResult>(getBooksResult);
            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);

            Assert.Equal("Ok", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Livros encontrados com sucesso", response.Message);
        }

        [Fact]
        public async Task GetBooks_ReturnBadRequest_WhenBookFilterIsNull()
        {
            var bookFilter = new BookFilterDTO
            {
                Title = "",
                Author = "",
                Category = "",
                Publisher = "",
                PublicationYear = null,
            };

            _bookRepositoryMock.Setup(service => service.GetBooksAsync(bookFilter))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.NullObject));

            var okSuccessResult = await _controller.Get(bookFilter);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(okSuccessResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O livro não pode ser nulo", response.Message);
        }

        [Fact]
        public async Task GetBooks_ReturnNotFound_WhenBooksNotFound()
        {
            var bookFilter = new BookFilterDTO
            {
                Title = "title",
                Author = "author",
                Category = "category",
                Publisher = "publisher",
                PublicationYear = 2000,
            };

            _bookRepositoryMock.Setup(service => service.GetBooksAsync(bookFilter))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.BookNotFound));

            var okSuccessResult = await _controller.Get(bookFilter);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(okSuccessResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Nenhum livro foi encontrado", response.Message);
        }

        [Fact]
        public async Task GetBookById_ReturnOk()
        {
            int validId = 1;

            var book = new BookReturnDTO
            {
                BookId = 1,
                Title = "title",
                Author = "author",
                Description = "description",
                Publisher = "publisher",
                PublicationYear = 2000,
                TotalCopies = 2,
                AvailableCopies = 1
            };

            _bookRepositoryMock.Setup(service => service.GetBookByIdAsync(validId))
                .ReturnsAsync(new RepositoryResponse<BookReturnDTO>(RepositoryStatus.Success, book));

            var getBookResult = await _controller.Get(validId);
            var okSuccessResult = Assert.IsType<OkObjectResult>(getBookResult);
            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);

            Assert.Equal("Ok", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal($"Livro de id '{validId}' encontrado com sucesso", response.Message);
        }

        [Fact]
        public async Task GetBookById_ReturnBadRequest_WhenIdIsInvalid()
        {
            int invalidId = 0;

            _bookRepositoryMock.Setup(service => service.GetBookByIdAsync(invalidId))
                .ReturnsAsync(new RepositoryResponse<BookReturnDTO>(RepositoryStatus.InvalidId));

            var getBooksResult = await _controller.Get(invalidId);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(getBooksResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O id '{invalidId}' não pode ser igual ou menor que 0", response.Message);
        }

        [Fact]
        public async Task GetBookById_ReturnNotFound_WhenBookNotFound()
        {
            int invalidId = 1;

            _bookRepositoryMock.Setup(service => service.GetBookByIdAsync(invalidId))
                .ReturnsAsync(new RepositoryResponse<BookReturnDTO>(RepositoryStatus.BookNotFound));

            var getBookResult = await _controller.Get(invalidId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(getBookResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O livro de id '{invalidId}' não foi encontrado", response.Message);
        }

        [Fact]
        public async Task GetAvailableBooks_ReturnOk()
        {
            var availableBooks = new List<BookReturnDTO>
            {
                new BookReturnDTO(),
                new BookReturnDTO()
            };

            _bookRepositoryMock.Setup(service => service.GetAvailableBooksAsync())
                .ReturnsAsync(new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.Success, availableBooks));

            var getBooksResult = await _controller.GetAvailableBooks();
            var okSuccessResult = Assert.IsType<OkObjectResult>(getBooksResult);
            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);

            Assert.Equal("Ok", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Livros disponíveis encontrados com sucesso", response.Message);
        }

        [Fact]
        public async Task GetAvailableBooks_ReturnNotFound_WhenBooksNotFound()
        {
            _bookRepositoryMock.Setup(service => service.GetAvailableBooksAsync())
                .ReturnsAsync(new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.BookNotFound));

            var getBooksResult = await _controller.GetAvailableBooks();
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(getBooksResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Nenhum livro disponível foi encontrado", response.Message);
        }

        [Fact]
        public async Task GetBorrowedBooks_ReturnOk()
        {
            var borrowedBooks = new List<BookReturnDTO>
            {
                new BookReturnDTO(),
                new BookReturnDTO()
            };

            _bookRepositoryMock.Setup(service => service.GetBorrowedBooksAsync())
                .ReturnsAsync(new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.Success, borrowedBooks));

            var getBooksResult = await _controller.GetBorrowedBooks();
            var okSuccessResult = Assert.IsType<OkObjectResult>(getBooksResult);
            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);

            Assert.Equal("Ok", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Livros emprestados encontrados com sucesso", response.Message);
        }

        [Fact]
        public async Task GetBorrowedBooks_ReturnNotFound_WhenBooksNotFound()
        {
            _bookRepositoryMock.Setup(service => service.GetBorrowedBooksAsync())
                .ReturnsAsync(new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.BookNotFound));

            var getBooksResult = await _controller.GetBorrowedBooks();
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(getBooksResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Nenhum livro emprestado foi encontrado", response.Message);
        }
    }
}
