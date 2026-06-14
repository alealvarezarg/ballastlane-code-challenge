namespace TaskManagementSystem.Infrastructure.Database;

public sealed class LiteDbSettings
{
    public const string SectionName = "LiteDb";

    public string ConnectionString { get; set; } = "Filename=task-management.db;Connection=shared";

    public string TasksCollectionName { get; set; } = "management_tasks";

    public string UsersCollectionName { get; set; } = "management_users";
}
