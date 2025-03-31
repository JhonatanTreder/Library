﻿using API.DTO.Event;
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
            if (eventCreateDTO is null)
            {
                return BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O evento não pode ser nulo"
                });
            }

            var _event = await _eventRepository.AddEventAsync(eventCreateDTO);

            return _event.Status switch 
            {
                RepositoryStatus.Success => CreatedAtAction(nameof(Get), new { id = _event.Data!.Id}, new ApiResponse
                {
                    Status = "Created",
                    Data = _event.Data,
                    Message = "Evento criado com sucesso"
                }),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O evento não pode ser nulo"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar criar um evento"
                })
            };
        }

        [HttpGet]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> Get([FromQuery] EventFilterDTO eventFilterDTO)
        {
            if (eventFilterDTO is null)
            {
                return BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O evento não pode ser nulo"
                });
            }

            var _events = await _eventRepository.GetEventsAsync(eventFilterDTO);

            return _events.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = _events.Data,
                    Message = "Eventos encontrados com sucesso"
                }),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O evento não pode ser nulo"
                }),

                RepositoryStatus.NotFound => NotFound(new ApiResponse 
                {
                    Status = "Not Found",
                    Data = null,
                    Message = "Nenhum evento foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar buscar os eventos"
                })
            };
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
                    Data = null,
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
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = $"O evento de id '{id}' não pode ser nulo"
                }),

                RepositoryStatus.NotFound => NotFound(new ApiResponse 
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O evento de id '{id}' não foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
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
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O evento de id '{id}' não foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar deletar um evento"
                })
            }; 
        }
    }
}
