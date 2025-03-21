using API.DTO.Book;
using API.DTO.Responses;
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
            var updated = await _bookRepository.UpdateBookAsync(id, bookUpdateDTO);

            if (updated is false)
            {
                return NotFound(new ApiResponse 
                {
                    Status = "Not Found",
                    Message = $"Livro de id '{id}' não encontrado"
                });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _bookRepository.DeleteBookAsync(id);

            if (deleted is false)
            {
                return NotFound(new ApiResponse 
                {
                    Status = "Not Found",
                    Message = $"Livro de id '{id}' não encontrado"
                });
            }

            return NoContent();
        }
    }
}
