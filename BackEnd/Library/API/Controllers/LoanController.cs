using API.DTO.Loan;
using API.DTO.Responses;
using API.Enum.Responses;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanRepository _loanRepository;
        public LoanController(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user,librarian,admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _loanRepository.GetLoanByIdAsync(id);

            return response.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = response,
                    Message = $"O empréstimo de id '{id}' foi encontrado com sucesso"
                }),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O empréstimo de id '{id}' não  foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = $"Erro inesperado ao obter o empréstimo de id '{id}'"
                })
            };
        }

        [HttpGet]
        [Authorize(Roles = "user,librarian,admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] LoanFilterDTO loanFilterDTO)
        {
            var loans = await _loanRepository.GetLoansAsync(loanFilterDTO);

            return loans.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = loans.Data,
                    Message = "Empréstimos encontrados com sucesso"
                }),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O empréstimo não pode ser nulo"
                }),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = "Nenhum empréstimo foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao buscar os empréstimos"
                })
            };
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "librarian")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromBody] LoanUpdateDTO loanUpdateDTO)
        {
            var response = await _loanRepository.UpdateLoanAsync(id, loanUpdateDTO);

            return response switch
            {
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O empréstimo não pode ser nulo"
                }),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O empréstimo de id '{id}' não foi encontrado"
                }),

                RepositoryStatus.BookNotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O livro que corresponde ao empréstimo de id '{id}' não foi encontrado"
                }),

                RepositoryStatus.InvalidStatusTransition => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "O novo status fornecido é inválido"
                }),

                RepositoryStatus.InvalidReturnDate => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "A nova data fornecida é inválida"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao atualizar um empréstimo"
                })
            };
        }

        [HttpPost]
        [Authorize(Roles = "librarian")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreateLoanDTO createLoanDTO)
        {
            var response = await _loanRepository.AddLoanAsync(createLoanDTO);

            return response.Status switch 
            {
                RepositoryStatus.Success => CreatedAtAction(nameof(Get), new { id = response.Data!.Id }, new ApiResponse
                {
                    Status = "Created",
                    Data = response.Data,
                    Message = "Empréstimo criado com sucesso"
                }),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O novo empréstimo não pode ser nulo"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar criar um empréstimo"
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
            var response = await _loanRepository.DeleteLoanAsync(id);

            return response switch
            {
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O empréstimo de id '{id}' não foi encontrado"
                }),

                RepositoryStatus.CannotDelete => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "Não é possível deletar um empréstimo com o status 'pending' ou 'in progress'"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao deletar um empréstimo"
                })
            };
        }

        [HttpGet("{id}/availability")]
        [Authorize(Roles = "user,librarian,admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookAvailability(int id)
        {
            var response = await _loanRepository.IsBookAvailableAsync(id);

            return response switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = null,
                    Message = $"O livro de id '{id}' está disponível para empréstimo"
                }),

                RepositoryStatus.BookNotAvailable => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = $"O livro de id '{id}' não está disponível para empréstimo no momento."
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro ao verificar a disponibilidade do livro."
                })
            };
        }

        [HttpPut("{id}/register-return")]
        [Authorize(Roles = "librarian")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id)
        {
            var response = await _loanRepository.RegisterReturnAsync(id);

            return response switch
            {
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O empréstimo de id '{id}' não foi encontrado"
                }),

                RepositoryStatus.InvalidStatus => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "O status do empréstimo deve estar em 'in progress' para ser registrado como 'finished'"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar registrar a devolução do empréstimo"
                })
            };
        }

        [HttpPut("{id}/extend-loan")]
        [Authorize(Roles = "librarian")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromBody] DateTime newDate)
        {
            var response = await _loanRepository.ExtendLoanAsync(id, newDate);

            return response switch
            {
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.InvalidDate => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "A nova data de devolução não pode ser menor ou igual a data de devolução antiga"
                }),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O empréstimo de id '{id}' não foi encontrado"
                }),

                RepositoryStatus.InvalidStatus => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "O status do empréstimo deve estar em 'in progress' para poder extender o prazo de devolucao"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao prolongar a devolução do empréstimo"
                })
            };
        }
    }
}
