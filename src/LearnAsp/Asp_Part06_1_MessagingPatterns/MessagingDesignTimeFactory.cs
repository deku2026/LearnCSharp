using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Part06_1_MessagingPatterns;

public sealed class MessagingDesignTimeFactory
    : IDesignTimeDbContextFactory<MessagingDbContext>
{
    public MessagingDbContext CreateDbContext(string[] args)
    {
        string connectionString =
            Environment.GetEnvironmentVariable("CAMPUS_W7_PG") ??
            "Host=localhost;Port=5432;Database=campus_w7_patterns;Username=dotnet;Password=dotnet_dev";
        DbContextOptions<MessagingDbContext> options = new DbContextOptionsBuilder<MessagingDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        return new MessagingDbContext(options);
    }
}
