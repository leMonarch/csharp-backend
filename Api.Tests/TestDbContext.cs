using Microsoft.EntityFrameworkCore;
using Api.Data;

namespace Api.Tests;

public static class TestDbContext
{
    public static AppDbContext CreateInMemory(string dbName = "TestDb")
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        var ctx = new AppDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }
}
