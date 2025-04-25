using API.Repositories;
using ApiUnitTests.Fixtures.Global;

namespace ApiUnitTests.Fixtures.Repositories
{
    public class BookRepositoryFixture
    {
        public BookRepository BookRepository { get; }

        public BookRepositoryFixture(DatabaseFixture dbFixture)
        {
            BookRepository = new BookRepository(dbFixture.DbContext);
        }
    }
}
