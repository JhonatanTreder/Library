using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Fixtures.Collections
{
    [CollectionDefinition("AuthServiceCollection")]
    public class AuthServiceCollection : ICollectionFixture<AuthServiceFixture>
    {
    }
}
