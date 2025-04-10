using API.Repositories;
using ApiUnitTests.Fixtures.Global;

namespace ApiUnitTests.Fixtures.Repositories
{
    public class LoanRepositoryFixture
    {
        public LoanRepository LoanRepository { get; }

        public LoanRepositoryFixture(DatabaseFixture dataBaseFixture)
        {
            LoanRepository = new LoanRepository(dataBaseFixture.DbContext);
        }
    }
}
