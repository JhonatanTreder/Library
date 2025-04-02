using API.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Fixtures
{
    class DataBaseFixture : IDisposable
    {
        public AppDbContext DbContext { get; }

        public DataBaseFixture()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDatabase").Options;

            DbContext = new AppDbContext(options);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
