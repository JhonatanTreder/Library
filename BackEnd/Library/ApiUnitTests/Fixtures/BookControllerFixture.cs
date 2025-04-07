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
    public class BookControllerFixture
    {
        public Mock<IBookRepository> BookRepositoryMock { get; }
        public BookController Controller { get; }

        public BookControllerFixture()
        {
            BookRepositoryMock = new Mock<IBookRepository>();
            Controller = new BookController(BookRepositoryMock.Object);
        }
    }
}
