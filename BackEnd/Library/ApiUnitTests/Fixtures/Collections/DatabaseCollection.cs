using ApiUnitTests.Fixtures.Global;

namespace ApiUnitTests.Fixtures.Collections
{
    [CollectionDefinition("DatabaseCollection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    { }
}
