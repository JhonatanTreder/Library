using ApiUnitTests.Fixtures.Global;

namespace ApiUnitTests.Collections
{
    [CollectionDefinition("DatabaseCollection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    { }
}
