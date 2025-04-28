using ApiUnitTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Collections
{
    [CollectionDefinition("LoanControllerCollection")]
    public class LoanControllerCollection : ICollectionFixture<LoanControllerFixture>
    { }
}
