using API.Repositories;
using ApiUnitTests.Fixtures.Global;

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
