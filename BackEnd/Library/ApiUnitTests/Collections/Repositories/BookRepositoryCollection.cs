using ApiUnitTests.Fixtures.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Collections.Repositories
{
    [CollectionDefinition("BookRepositoryCollection")]
    public class BookRepositoryCollection : ICollectionFixture<BookRepositoryFixture>
    { }
}
