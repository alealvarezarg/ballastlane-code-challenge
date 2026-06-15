namespace TaskManagementSystem.Api.UnitTests;

internal static class TestPathHelper
{
    public static string GetWorkspaceRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "AGENTS.md")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Workspace root could not be located.");
    }

    public static string GetApiRoot()
    {
        var workspaceRoot = GetWorkspaceRoot();
        var apiRoot = Path.Combine(workspaceRoot, "api");

        if (Directory.Exists(apiRoot) && File.Exists(Path.Combine(apiRoot, "TaskManagementSystem.slnx")))
        {
            return apiRoot;
        }

        throw new InvalidOperationException("API root could not be located.");
    }
}
