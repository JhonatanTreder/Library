using API.Repositories;
using ApiUnitTests.Fixtures.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
