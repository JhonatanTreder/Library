using API.DTO.Event;
using API.DTO.Responses;
using API.Enum.Responses;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        [HttpPost]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Post([FromBody] EventCreateDTO eventCreateDTO)
        {
            var _event = await _eventRepository.AddEventAsync(eventCreateDTO);

            if (_event is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Falha ao tentar criar um evento"
                });
            }

            return CreatedAtAction(nameof(Get), new { id = _event.Id }, new ApiResponse
            {
                Status = "Created",
                Data = _event,
                Message = "Evento criado com sucesso"
            });
        }

        [HttpGet]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> Get([FromQuery] EventFilterDTO eventFilterDTO)
        {
            var _events = await _eventRepository.GetEventsAsync(eventFilterDTO);

            if (_events is null || !_events.Any())
            {
                return NotFound(new ApiResponse 
                {
                    Status = "Not Found",
                    Message = "Nenhum evento foi encontrado"
                });
            }

            return Ok(new ApiResponse 
            {
                Status = "Ok",
                Data = _events,
                Message = "Eventos encontrados com sucesso"
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> Get(int id)
        {
            var _event = await _eventRepository.GetEventByIdAsync(id);

            if (_event is null)
            {
                return NotFound(new ApiResponse 
                {
                    Status = "Not Found",
                    Message = $"O evento de id '{id}' não foi encontrado"
                });
            }

            return Ok(new ApiResponse 
            {
                Status = "Ok",
                Data = _event,
                Message = $"Evento de id '{id}' encontrado com sucesso"
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Put(int id, [FromBody] EventUpdateDTO eventUpdateDTO)
        {
            var response = await _eventRepository.UpdateEventAsync(id, eventUpdateDTO);

            return response switch
            {
                EventResponse.Success => NoContent(),

                EventResponse.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Message = $"O evento de id '{id}' não pode ser nulo"
                }),

                EventResponse.NotFound => NotFound(new ApiResponse 
                {
                    Status = "Not Found",
                    Message = $"O evento de id '{id}' não foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Erro inesperado ao tentar atualizar um evento"
                })
            };
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "librarian")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _eventRepository.DeleteEventAsync(id);

            return response switch 
            {
                EventResponse.Success => NoContent(),

                EventResponse.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"O evento de id '{id}' não foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Erro inesperado ao tentar deletar um evento"
                })
            }; 
        }
    }
}
