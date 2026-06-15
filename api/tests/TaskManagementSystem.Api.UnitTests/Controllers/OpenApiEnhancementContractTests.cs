using Shouldly;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class OpenApiEnhancementContractTests
{
    [Fact]
    public void CanonicalContract_ShouldContainEnhancedManagementTaskRoutes()
    {
        var workspaceRoot = TestPathHelper.GetWorkspaceRoot();
        var canonicalContract = Path.Combine(workspaceRoot, "openapi", "openapi.yaml");
        var content = File.ReadAllText(canonicalContract).Replace("\r\n", "\n");

        content.ShouldContain("/management-tasks/summary");
        content.ShouldContain("/management-tasks/overdue");
        content.ShouldContain("/management-tasks/due-within");
        content.ShouldContain("/management-tasks/{id}/status");
        content.ShouldContain("PatchManagementTaskRequestDto");
        content.ShouldContain("/management-users/login");
        content.ShouldNotContain("/management-users/{id}");
        content.ShouldNotContain("sortBy");
        content.ShouldNotContain("sortDirection");
        content.ShouldContain("BearerAuth");
    }
}
