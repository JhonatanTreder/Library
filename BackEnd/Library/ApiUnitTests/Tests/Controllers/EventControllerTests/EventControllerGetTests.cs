using API.Controllers;
using API.DTO.Event;
using API.DTO.Responses;
using API.Enum.Responses;
using API.Models;
using API.Repositories.Interfaces;
using ApiUnitTests.Fixtures.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Tests.Controllers.EventControllerTests
{
    [Collection("EventControllerCollection")]
    public class EventControllerGetTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly EventController _controller;

        public EventControllerGetTests(EventControllerFixture fixture)
        {
            _eventRepositoryMock = fixture.EventRepositoryMock;
            _controller = fixture.Controller;
        }

        [Fact]
        public async Task GetEvents_ReturnOk()
        {
            var eventFilter = new EventFilterDTO 
            {
                Title = "valid-title",
                TargetAudience = "valid-target-audience",
                Location = "valid-location",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(2)
            };

            var events = new List<Event> 
            {
                new Event(),
                new Event(),
                new Event()
            };

            _eventRepositoryMock.Setup(service => service.GetEventsAsync(eventFilter))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<Event>>(RepositoryStatus.Success, events));

            var getResult = await _controller.Get(eventFilter);
            var okSuccessResult = Assert.IsType<OkObjectResult>(getResult);
            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);

            Assert.Equal("Ok", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Eventos encontrados com sucesso", response.Message);
        }

        [Fact]
        public async Task GetEvents_ReturnBadRequest_WhenEventIsNull()
        {
            var eventFilter = new EventFilterDTO();

            _eventRepositoryMock.Setup(service => service.GetEventsAsync(eventFilter))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<Event>>(RepositoryStatus.NullObject));

            var getResult = await _controller.Get(eventFilter);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(getResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O evento não pode ser nulo", response.Message);
        }

        [Fact]
        public async Task GetEvents_ReturnNotFound_WhenEventsNotFound()
        {
            var eventFilter = new EventFilterDTO();

            _eventRepositoryMock.Setup(service => service.GetEventsAsync(eventFilter))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<Event>>(RepositoryStatus.NotFound));

            var getResult = await _controller.Get(eventFilter);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(getResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Nenhum evento foi encontrado", response.Message);
        }

        [Fact]
        public async Task GetEventById_ReturnOk()
        {
            int validId = 1;

            var _event = new Event();

            _eventRepositoryMock.Setup(service => service.GetEventByIdAsync(validId))
                .ReturnsAsync(new RepositoryResponse<Event>(RepositoryStatus.Success, _event));

            var getResult = await _controller.Get(validId);
            var okSuccessResult = Assert.IsType<OkObjectResult>(getResult);
            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);

            Assert.Equal("Ok", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal($"Evento de id '{validId}' encontrado com sucesso", response.Message);
        }

        [Fact]
        public async Task GetEventById_ReturnNotFound_WhenEventNotFound()
        {
            int eventId = 1;

            _eventRepositoryMock.Setup(service => service.GetEventByIdAsync(eventId))
                .ReturnsAsync(new RepositoryResponse<Event>(RepositoryStatus.NotFound));

            var getResult = await _controller.Get(eventId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(getResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O evento de id '{eventId}' não foi encontrado", response.Message);
        }
    }
}
