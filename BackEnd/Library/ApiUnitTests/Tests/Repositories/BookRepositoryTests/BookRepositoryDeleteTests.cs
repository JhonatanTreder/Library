using ApiUnitTests.Fixtures.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Tests.Repositories.BookRepositoryTests
{
    [Collection("DatabaseCollection")]
    public class BookRepositoryDeleteTests
    {
        private readonly BookRepositoryFixture _fixture;

        public BookRepositoryDeleteTests(BookRepositoryFixture fixture)
        {
            _fixture = fixture;
        }
    }
}
