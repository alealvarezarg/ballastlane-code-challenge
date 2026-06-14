using Shouldly;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class OpenApiEnhancementContractTests
{
    [Fact]
    public void CanonicalContract_ShouldContainEnhancedManagementTaskRoutes()
    {
        var repositoryRoot = TestPathHelper.GetRepositoryRoot();
        var canonicalContract = Path.Combine(repositoryRoot, "openapi", "openapi.yaml");
        var content = File.ReadAllText(canonicalContract).Replace("\r\n", "\n");

        content.ShouldContain("/management-tasks/summary");
        content.ShouldContain("/management-tasks/overdue");
        content.ShouldContain("/management-tasks/due-within");
        content.ShouldContain("/management-tasks/{id}/status");
        content.ShouldContain("PatchManagementTaskRequestDto");
    }
}
