using API.Controllers;
using API.Repositories.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Fixtures
{
    public class LoanControllerFixture
    {
        public Mock<ILoanRepository> LoanRepositoryMock;
        public LoanController Controller;

        public LoanControllerFixture()
        {
            LoanRepositoryMock = new Mock<ILoanRepository>();
            Controller = new LoanController(LoanRepositoryMock.Object);
        }
    }
}
