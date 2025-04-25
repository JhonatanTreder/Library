using API.Context;
using API.Repositories;
using ApiUnitTests.Fixtures.Global;

namespace ApiUnitTests.Fixtures.Repositories
{
    public class BookRepositoryFixture
    {
        public BookRepository BookRepository { get; }
        public AppDbContext DbContext { get; }

        public BookRepositoryFixture(DatabaseFixture dbFixture)
        {
            DbContext = dbFixture.DbContext;
            BookRepository = new BookRepository(DbContext);
        }
    }
}
