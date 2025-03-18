using API.DTO.Loan;
using API.DTO.Responses;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace API.Controllers
{
    [Route("api/[controller]")]
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
                Message = $"O empréstimo de id '{id}' foi encontrado com sucesso"
            });
        }

        [HttpGet]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> Get([FromBody] LoanFilterDTO loanFilterDTO)
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
                Message = "Empréstimos encontrados com sucesso"
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Update(int id, [FromBody] LoanUpdateDTO loanUpdateDTO)
        {
            var updated = await _loanRepository.UpdateLoanAsync(id, loanUpdateDTO);

            if (!updated)
            {
                return NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"Empréstimo de id '{id}' não encontrado"
                });
            }

            return NoContent();
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
                    Message = "Erro inesperado ao criar um empréstimo"
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
            var deleted = await _loanRepository.DeleteLoanAsync(id);

            if (!deleted)
            {
                return NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"Empréstimo de id '{id}' não encontrado"
                });
            }

            return NoContent();
        }
    }
}
