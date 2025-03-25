using API.DTO.Loan;
using API.DTO.Responses;
using API.Enum.Responses;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Reflection.Metadata.Ecma335;

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
        public async Task<IActionResult> Get(int id)
        {
            var loan = await _loanRepository.GetLoanByIdAsync(id);

            if (loan is null)
            {
                return NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"O empréstimo de id '{id}' não  foi encontrado"
                });
            }

            return Ok(new ApiResponse
            {
                Status = "Ok",
                Data = loan,
                Message = $"O empréstimo de id '{id}' foi encontrado com sucesso"
            });
        }

        [HttpGet]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> Get([FromQuery] LoanFilterDTO loanFilterDTO)
        {
            var loans = await _loanRepository.GetLoansAsync(loanFilterDTO);

            if (loans is null || !loans.Any())
            {
                return NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = "Nenhum empréstimo foi encontrado"
                });
            }

            return Ok(new ApiResponse
            {
                Status = "Ok",
                Data = loans,
                Message = "Empréstimos encontrados com sucesso"
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Put(int id, [FromBody] LoanUpdateDTO loanUpdateDTO)
        {
            var response = await _loanRepository.UpdateLoanAsync(id, loanUpdateDTO);

            return response switch
            {
                LoanResponse.Success => NoContent(),

                LoanResponse.NotFound => NotFound(new ApiResponse 
                {
                    Status = "Not Found",
                    Message = $"O empréstimo de id '{id}' não foi encontrado"
                }),

                LoanResponse.BookNotFound => NotFound(new ApiResponse 
                {
                    Status = "Not Found",
                    Message = $"O livro que corresponde ao empréstimo de id '{id}' não foi encontrado"
                }),

                LoanResponse.InvalidStatusTransition => Conflict(new ApiResponse 
                {
                    Status = "Conflict",
                    Message = "O novo status fornecido é inválido"
                }),

                LoanResponse.InvalidReturnDate => Conflict(new ApiResponse 
                {
                    Status = "Conflict",
                    Message = "A nova data fornecida é inválida"
                }),

                LoanResponse.NullObject => StatusCode(StatusCodes.Status500InternalServerError , new ApiResponse 
                {
                    Status = "Internal Server Error",
                    Message = "O empréstimo não pode ser nulo"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse 
                {
                    Status = "Internal Server Error",
                    Message = "Erro inesperado ao atualizar um empréstimo"
                })
            };
        }

        [HttpPost]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Post([FromBody] CreateLoanDTO createLoanDTO)
        {
            var loan = await _loanRepository.AddLoanAsync(createLoanDTO);

            if (loan is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Erro inesperado ao tentar criar um empréstimo"
                });
            }

            return CreatedAtAction(nameof(Get), new { id = loan.Id }, new ApiResponse
            {
                Status = "Created",
                Data = loan,
                Message = "Empréstimo criado com sucesso"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _loanRepository.DeleteLoanAsync(id);

            return response switch
            {
                LoanResponse.Success => NoContent(),

                LoanResponse.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"O empréstimo de id '{id}' não foi encontrado"
                }),

                LoanResponse.CannotDelete => Conflict(new ApiResponse 
                {
                    Status = "Conflict",
                    Message = "Não é possível deletar um empréstimo com o status 'pending' ou 'in progress'"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse 
                {
                    Status = "Internal Server Error",
                    Message = "Erro inesperado ao deletar um empréstimo"
                })
            };
        }

        [HttpGet("{id}/availability")]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> GetBookAvailability(int id)
        {
            var response = await _loanRepository.IsBookAvailableAsync(id);

            return response switch
            {
                LoanResponse.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Message = $"O livro de id '{id}' está disponível para empréstimo"
                }),

                LoanResponse.BookNotAvailable => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Message = $"O livro de id '{id}' não está disponível para empréstimo no momento."
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Erro ao verificar a disponibilidade do livro."
                })
            };
        }

        [HttpPut("{id}/register-return")]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Put(int id)
        {
            var response = await _loanRepository.RegisterReturnAsync(id);

            return response switch
            {
                LoanResponse.Success => NoContent(),

                LoanResponse.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"O empréstimo de id '{id}' não foi encontrado"
                }),

                LoanResponse.InvalidStatus => Conflict(new ApiResponse 
                {
                    Status = "Conflict",
                    Message = "O status do empréstimo deve estar em 'in progress' para ser registrado como 'finished'"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Erro inesperado ao tentar registrar a devolução do empréstimo"
                })
            };
        }

        [HttpPut("{id}/extend-loan")]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Put(int id, [FromBody] DateTime newDate)
        {
            var response = await _loanRepository.ExtendLoanAsync(id, newDate);

            return response switch 
            {
                LoanResponse.Success => NoContent(),

                LoanResponse.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"O empréstimo de id '{id}' não foi encontrado"
                }),

                LoanResponse.InvalidStatus => Conflict(new ApiResponse 
                {
                    Status = "Conflict",
                    Message = "O status do empréstimo deve estar em 'in progress' para poder extender o prazo de devolucao"
                }),

                LoanResponse.InvalidDate => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Message = "A nova data de devolução não pode ser menor ou igual a data de devolução antiga"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Erro inesperado ao prolongar a devolução do empréstimo"
                })
            };
        }
    }
}
