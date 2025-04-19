using API.Controllers;
using API.Repositories.Interfaces;
using ApiUnitTests.Fixtures;
using ApiUnitTests.Fixtures.Repositories;
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
    }
}
