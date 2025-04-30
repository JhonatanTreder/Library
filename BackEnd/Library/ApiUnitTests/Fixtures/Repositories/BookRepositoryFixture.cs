using API.Context;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApiUnitTests.Fixtures.Repositories
{
    public class BookRepositoryFixture : IDisposable
    {
        public BookRepository BookRepository { get; }
        public AppDbContext DbContext { get; }

        public BookRepositoryFixture()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
           .UseInMemoryDatabase(Guid.NewGuid().ToString())
           .Options;

            DbContext = new AppDbContext(options);
            BookRepository = new BookRepository(DbContext);
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}
