using API.Repositories;
using ApiUnitTests.Fixtures.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
