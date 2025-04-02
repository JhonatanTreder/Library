using API.Controllers;
using API.Services.Interfaces;
using ApiUnitTests.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Tests.Controllers.AuthControllerTests
{
    class AuthControllerGetTests
    {
        private readonly ServiceFixture _serviceFixture;

        public AuthControllerGetTests(ServiceFixture serviceFixture)
        {
            _serviceFixture = serviceFixture;
        }
    }
}
