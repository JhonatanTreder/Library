using ApiUnitTests.Fixtures.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Collections.Controllers
{
    [CollectionDefinition("EventControllerCollection")]
    public class EventControllerCollection : ICollectionFixture<EventControllerFixture>
    { }
}
