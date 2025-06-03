using API.Controllers;
using API.DTOs.Event;
using API.DTOs.Responses;
using API.Enum.Responses;
using API.Models;
using API.Repositories.Interfaces;
using ApiUnitTests.Fixtures.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Tests.Controllers.EventControllerTests
{
    [Collection("EventControllerCollection")]
    public class EventControllerPostTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly EventController _controller;

        public EventControllerPostTests(EventControllerFixture fixture)
        {
            _eventRepositoryMock = fixture.EventRepositoryMock;
            _controller = fixture.Controller;
        }

        [Fact]
        public async Task PostEvent_ReturnCreated()
        {
            var eventData = new EventCreateDTO 
            {
                Title = "valid-title",
                Description = "valid-description",
                TargetAudience = "valid-target-audience",
                Location = "valid-location",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(2)
            };

            var newEvent = new Event 
            {
                Title = eventData.Title,
                Description = eventData.Description,
                TargetAudience = eventData.TargetAudience,
                Location = eventData.Location,
                StartDate = eventData.StartDate,
                EndDate = eventData.EndDate
            };

            _eventRepositoryMock.Setup(service => service.AddEventAsync(eventData))
                .ReturnsAsync(new RepositoryResponse<Event>(RepositoryStatus.Success, newEvent));

            var postResult = await _controller.Post(eventData);
            var okSuccessResult = Assert.IsType<CreatedAtActionResult>(postResult);
            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);

            Assert.Equal("Created", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Evento criado com sucesso", response.Message);
        }

        [Fact]
        public async Task PostEvent_ReturnBadRequest_WhenEventIsNull()
        {
            var eventData = new EventCreateDTO();

            _eventRepositoryMock.Setup(service => service.AddEventAsync(eventData))
                .ReturnsAsync(new RepositoryResponse<Event>(RepositoryStatus.NullObject));

            var postResult = await _controller.Post(eventData);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(postResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O evento não pode ser nulo", response.Message);
        }
    }
}
