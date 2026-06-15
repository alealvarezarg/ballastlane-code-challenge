using Microsoft.Extensions.Options;
using Shouldly;
using TaskManagementSystem.Infrastructure.Database;

namespace TaskManagementSystem.Infrastructure.UnitTests.Database;

public sealed class LiteDbContextTests : IDisposable
{
    private readonly string _databasePath;

    public LiteDbContextTests()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.db");
    }

    [Fact]
    public void Constructor_ShouldExposeTasksAndUsersCollections_AndCreateDatabaseFile()
    {
        var settings = new LiteDbSettings
        {
            ConnectionString = $"Filename={_databasePath};Connection=shared"
        };

        using var context = new LiteDbContext(Options.Create(settings));

        context.Tasks.ShouldNotBeNull();
        context.Users.ShouldNotBeNull();
        File.Exists(_databasePath).ShouldBeTrue();
        context.Settings.ConnectionString.ShouldBe(settings.ConnectionString);
    }

    public void Dispose()
    {
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }
}
