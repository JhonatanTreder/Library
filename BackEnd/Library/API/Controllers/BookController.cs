using API.DTO.Book;
using API.DTO.Responses;
using API.Enum.Responses;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> Get([FromQuery] BookFilterDTO bookDTO)
        {
            var books = await _bookRepository.GetBooksAsync(bookDTO);

            return books.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = books.Data,
                    Message = "Livros encontrados com sucesso"
                }),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O livro não pode ser nulo"
                }),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = "Nenhum livro foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao buscar os livros"
                })
            };
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _bookRepository.GetBookByIdAsync(id);

            return response.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = response.Data,
                    Message = $"Livro de id '{id}' encontrado com sucesso"
                }),

                RepositoryStatus.InvalidId => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = $"O id '{id}' não pode ser igual ou menor que 0"
                }),

                RepositoryStatus.BookNotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O livro de id '{id}' não foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = $"Erro inesperado ao buscar o livro de id '{id}'"
                })
            };
        }

        [HttpGet("available")]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> GetAvailableBooks()
        {
            var books = await _bookRepository.GetAvailableBooksAsync();

            return books.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = books.Data,
                    Message = "Livros disponíveis encontrados com sucesso"
                }),

                RepositoryStatus.BookNotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = "Nenhum livro disponível foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao buscar por livros disponíveis"
                })
            };
        }

        [HttpGet("borrowed")]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            var books = await _bookRepository.GetBorrowedBooksAsync();

            return books.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = books.Data,
                    Message = "Livros emprestados encontrados com sucesso"
                }),

                RepositoryStatus.BookNotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = "Nenhum livro emprestado foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao buscar por livros emprestados"
                })
            };
        }

        [HttpPost]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Post([FromBody] CreateBookDTO bookDTO)
        {
            var book = await _bookRepository.AddBookAsync(bookDTO);

            return book.Status switch
            {
                RepositoryStatus.Success => CreatedAtAction(nameof(Get), new { id = book.Data!.Id }, new ApiResponse
                {
                    Status = "Created",
                    Data = book.Data,
                    Message = "Livro criado com sucesso"
                }),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Message = book.Status == RepositoryStatus.NullObject
                        ? "Os dados do livro são inválidos."
                        : "Erro inesperado ao tentar criar um livro."
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {

                })
            };
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Put(int id, [FromBody] BookUpdateDTO bookUpdateDTO)
        {
            var response = await _bookRepository.UpdateBookAsync(id, bookUpdateDTO);

            return response switch
            {
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "O livro não pode ser nulo"
                }),

                RepositoryStatus.InvalidQuantity => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "A quantidade de livros deve ser maior que zero"
                }),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "NotFound",
                    Data = null,
                    Message = $"O livro de id '{id}' não foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao atualizar o livro"
                })
            };
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _bookRepository.DeleteBookAsync(id);

            return response switch
            {
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.BookNotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O livro de id '{id}' não foi encontrado"
                }),

                RepositoryStatus.CannotDelete => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "Não é possível deletar um livro que o status está em progresso"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao deletar um livro"
                })
            };
        }
    }
}
