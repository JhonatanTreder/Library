using API.DTO.Book;
using API.DTO.Responses;
using API.Enum.Responses;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

            if (books is null || !books.Any())
            {
                return NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = "Nenhum livro foi encontrado"
                });
            }

            return Ok(new ApiResponse
            {
                Status = "Ok",
                Data = books,
                Message = "Livros encontrados com sucesso"
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> Get(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);

            if (book is null)
            {
                return NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"Livro de id '{id}' não encontrado"
                });
            }

            return Ok(new ApiResponse
            {
                Status = "Ok",
                Data = book,
                Message = $"Livro de id '{id}' localizado com sucesso"
            });
        }

        [HttpGet("available")]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> GetAvailableBooks()
        {
            var books = await _bookRepository.GetAvailableBooksAsync();

            if (books is null || !books.Any())
            {
                return NotFound(new ApiResponse 
                {
                    Status = "Not Found",
                    Message = "Nenhum livro disponível foi encontrado"
                });
            }

            return Ok(new ApiResponse 
            {
                Status = "Ok",
                Data = books,
                Message = "Livros disponíveis encontrados com sucesso"
            });
        }

        [HttpGet("borrowed")]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            var books = await _bookRepository.GetBorrowedBooksAsync();

            if (books is null || !books.Any())
            {
                return NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = "Nenhum livro emprestado foi encontrado"
                });
            }

            return Ok(new ApiResponse
            {
                Status = "Ok",
                Data = books,
                Message = "Livros emprestados encontrados com sucesso"
            });
        }

        [HttpPost]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Post([FromBody] CreateBookDTO bookDTO)
        {
            var book = await _bookRepository.AddBookAsync(bookDTO);

            if (book is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Erro inesperado ao criar um livro"
                });
            }

            return CreatedAtAction(nameof(Get), new { id = book.Id }, new ApiResponse
            {
                Status = "Created",
                Data = book,
                Message = "Livro criado com sucesso"
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Put(int id, [FromBody] BookUpdateDTO bookUpdateDTO)
        {
            var response = await _bookRepository.UpdateBookAsync(id, bookUpdateDTO);

            return response switch
            {
                BookResponse.Success => NoContent(),

                BookResponse.InvalidQuantity => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Message = "A quantidade de livros deve ser maior que zero"
                }),

                BookResponse.NotFound => NotFound(new ApiResponse
                {
                    Status = "NotFound",
                    Message = $"O livro de id '{id}' não foi encontrado"
                }),

                BookResponse.NullObject => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "O livro não pode ser nulo"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
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
                BookResponse.Success => NoContent(),

                BookResponse.NotFound => NotFound(new ApiResponse
                {
                    Status = "NotFound",
                    Message = $"O livro de id '{id}' não foi encontrado"
                }),

                BookResponse.CannotDelete => Conflict( new ApiResponse 
                {
                    Status = "Conlfict",
                    Message = "Não é possível deletar um livro que está em progresso"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse 
                {
                    Status = "Internal Server Error",
                    Message = "Erro inesperado ao deletar um livro"
                })
            };
        }
    }
}
