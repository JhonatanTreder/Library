﻿using API.Controllers;
using API.DTOs.Event;
using API.DTOs.Responses;
using API.Enum.Responses;
using API.Repositories.Interfaces;
using ApiUnitTests.Fixtures.Controllers;
using ApiUnitTests.Fixtures.Repositories;
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
    public class EventControllerPutTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly EventController _controller;

        public EventControllerPutTests(EventControllerFixture fixture)
        {
            _eventRepositoryMock = fixture.EventRepositoryMock;
            _controller = fixture.Controller;
        }

        [Fact]
        public async Task PutEvent_ReturnNoContent()
        {
            int validId = 1;

            var newEvent = new EventUpdateDTO 
            {
                Title = "valid-title",
                Description = "valid-description",
                TargetAudience = "valid-target-atudience",
                Location = "valid-location",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(2)
            };

            _eventRepositoryMock.Setup(service => service.UpdateEventAsync(validId, newEvent))
                .ReturnsAsync(RepositoryStatus.Success);

            var updateResult = await _controller.Put(validId, newEvent);
            Assert.IsType<NoContentResult>(updateResult);
        }

        [Fact]
        public async Task PutEvent_ReturnBadRequest_WhenEventIsNull()
        {
            int eventId = 1;

            var newEvent = new EventUpdateDTO();

            _eventRepositoryMock.Setup(service => service.UpdateEventAsync(eventId, newEvent))
                .ReturnsAsync(RepositoryStatus.NullObject);

            var updateResult = await _controller.Put(eventId, newEvent);
            var noContentResult = Assert.IsType<BadRequestObjectResult>(updateResult);
            var response = Assert.IsType<ApiResponse>(noContentResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O evento de id '{eventId}' não pode ser nulo", response.Message);
        }

        [Fact]
        public async Task PutEvent_ReturnNotFound_WhenEventNotFound()
        {
            int eventId = 1;

            var newEvent = new EventUpdateDTO
            {
                Title = "valid-title",
                Description = "valid-description",
                TargetAudience = "valid-target-atudience",
                Location = "valid-location",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(2)
            };

            _eventRepositoryMock.Setup(service => service.UpdateEventAsync(eventId, newEvent))
                .ReturnsAsync(RepositoryStatus.NotFound);

            var updateResult = await _controller.Put(eventId, newEvent);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(updateResult);

            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O evento de id '{eventId}' não foi encontrado", response.Message);
        }
    }
}
