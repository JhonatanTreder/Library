using API.Controllers;
using API.Repositories.Interfaces;
using Moq;

namespace ApiUnitTests.Fixtures.Controllers
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
