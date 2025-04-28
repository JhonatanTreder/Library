using API.Controllers;
using API.Repositories.Interfaces;
using ApiUnitTests.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Tests.Controllers.LoanControllerTests
{
    [Collection("LoanControllerCollection")]
    public class LoanControllerDeleteTests
    {
        private readonly Mock<ILoanRepository> _loanRepositoryMock;
        private readonly LoanController _controller;

        public LoanControllerDeleteTests(LoanControllerFixture fixture)
        {
            _loanRepositoryMock = fixture.LoanRepositoryMock;
            _controller = fixture.Controller;
        }
    }
}
