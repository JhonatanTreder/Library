﻿using API.Context;
using Microsoft.EntityFrameworkCore;

namespace ApiUnitTests.Fixtures.Global
{
    public class DatabaseFixture : IDisposable
    {
        public AppDbContext DbContext { get; }

        public DatabaseFixture()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDatabase").Options;

            DbContext = new AppDbContext(options);
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}
