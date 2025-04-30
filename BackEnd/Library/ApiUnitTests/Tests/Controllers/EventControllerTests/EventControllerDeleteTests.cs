using API.Controllers;
using API.DTO.Responses;
using API.Enum.Responses;
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
    public class EventControllerDeleteTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly EventController _controller;

        public EventControllerDeleteTests(EventControllerFixture fixture)
        {
            _eventRepositoryMock = fixture.EventRepositoryMock;
            _controller = fixture.Controller;
        }

        [Fact]
        public async Task DeleteEvent_ReturnNoContent()
        {
            int validId = 1;

            _eventRepositoryMock.Setup(service => service.DeleteEventAsync(validId))
                .ReturnsAsync(RepositoryStatus.Success);

            var deleteResult = await _controller.Delete(validId);
            Assert.IsType<NoContentResult>(deleteResult);
        }

        [Fact]
        public async Task DeleteEvent_ReturnNotFound_WhenEventNotFound()
        {
            int eventId = 1;

            _eventRepositoryMock.Setup(service => service.DeleteEventAsync(eventId))
                .ReturnsAsync(RepositoryStatus.NotFound);

            var deleteResult = await _controller.Delete(eventId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(deleteResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O evento de id '{eventId}' não foi encontrado", response.Message);
        }
    }
}
