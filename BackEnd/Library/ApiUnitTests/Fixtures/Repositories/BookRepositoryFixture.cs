using API.Repositories;
using ApiUnitTests.Fixtures.Global;

namespace ApiUnitTests.Fixtures.Repositories
{
    class BookRepositoryFixture
    {
        public BookRepository BookRepository { get; }

        public BookRepositoryFixture(DatabaseFixture dataBaseFixture)
        {
            BookRepository = new BookRepository(dataBaseFixture.DbContext);
        }
    }
}
