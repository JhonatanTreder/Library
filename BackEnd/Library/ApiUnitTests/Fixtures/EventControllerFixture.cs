using API.Controllers;
using API.Repositories.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Fixtures
{
    public class EventControllerFixture
    {
        public Mock<IEventRepository> EventRepositoryMock { get; }
        public EventController Controller { get; }

        public EventControllerFixture()
        {
            EventRepositoryMock = new Mock<IEventRepository>();
            Controller = new EventController(EventRepositoryMock.Object);
        }
    }
}
