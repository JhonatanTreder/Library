using API.Repositories;
using ApiUnitTests.Fixtures.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Fixtures.Repositories
{
    public class EventRepositoryFixture
    {
        public EventRepository EventRepository { get; }

        public EventRepositoryFixture(DatabaseFixture dbFixture)
        {
            EventRepository = new EventRepository(dbFixture.DbContext);
        }
    }
}
