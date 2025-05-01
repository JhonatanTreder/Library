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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        [HttpGet("{bookId}/copies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookCopies(int bookId)
        {
            var response = await _bookRepository.GetBookCopiesAsync(bookId);

            return response.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = response.Data,
                    Message = $"As cópias do livro de id '{bookId}' foram encontradas com sucesso"
                }),

                RepositoryStatus.InvalidId => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = $"O id '{bookId}' está em um formato inválido"
                }),

                RepositoryStatus.BookNotFound => NotFound(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = $"O livro de id '{bookId}' não foi encontrado"
                }),

                RepositoryStatus.BookCopyNotFound => NotFound(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = $"A cópia referente ao livro de id '{bookId}' não foi encontrada"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = $"Erro inesperado ao buscar pelas cópias do livro de id '{bookId}'"
                })
            };
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user,librarian,admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        [HttpPost("{bookId}")]
        [Authorize(Roles = "librarian")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post(int bookId)
        {
            var response = await _bookRepository.AddBookCopyAsync(bookId);

            return response.Status switch
            {
                RepositoryStatus.Success => CreatedAtAction(nameof(Get), new { id = response.Data!.BookId }, new ApiResponse 
                {
                    Status = "Created",
                    Data = response.Data,
                    Message = $"Cópia do livro de id '{response.Data.BookId}' criada com sucesso"
                }),

                RepositoryStatus.InvalidId => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = $"O id '{bookId}' está em um formato inválido"
                }),

                RepositoryStatus.BookNotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O livro referente ao id '{bookId}' não foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse 
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = $"Erro inesperado ao tentar criar uma cópia do livro de id '{bookId}'"
                })
            };
        }

        [HttpPost]
        [Authorize(Roles = "librarian")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreateBookDTO bookDTO)
        {
            var book = await _bookRepository.AddBookAsync(bookDTO);

            return book.Status switch
            {
                RepositoryStatus.Success => CreatedAtAction(nameof(Get), new { id = book.Data!.BookId }, new ApiResponse
                {
                    Status = "Created",
                    Data = book.Data,
                    Message = "Livro criado com sucesso"
                }),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O livro não pode ser nulo"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar criar um livro"
                })
            };
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "librarian")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromBody] BookUpdateDTO bookUpdateDTO)
        {
            var response = await _bookRepository.UpdateBookAsync(id, bookUpdateDTO);

            return response switch
            {
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O livro não pode ser nulo"
                }),

                RepositoryStatus.InvalidQuantity => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "A nova quantidade de livros deve ser maior que zero"
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
                    Message = "Erro inesperado ao atualizar o livro"
                })
            };
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "librarian")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        [HttpDelete("{bookId}/copies/{copyId}")]
        [Authorize(Roles = "user,librarian,admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int bookId, int copyId)
        {
            var status = await _bookRepository.DeleteBookCopyAsync(bookId, copyId);

            return status switch
            {
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.InvalidId => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Message = "O id do livro ou de uma cópia estão inválidos"
                }),

                RepositoryStatus.BookNotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"O livro de id '{bookId}' não foi encontrado"
                }),

                RepositoryStatus.BookCopyNotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"A cópia de id '{copyId}' não foi encontrada"
                }),

                RepositoryStatus.BookCopyDoesNotBelongToBook => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Message = $"A cópia de id '{copyId}' não pertence ao livro '{bookId}'"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Erro inesperado ao deletar a cópia do livro"
                })
            };
        }
    }
}
