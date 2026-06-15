using Shouldly;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class OpenApiContractTests
{
    [Fact]
    public void CanonicalAndMirroredContracts_ShouldStayInSync()
    {
        var workspaceRoot = TestPathHelper.GetWorkspaceRoot();
        var canonicalContract = Path.Combine(workspaceRoot, "openapi", "openapi.yaml");
        var mirroredContract = Path.Combine(workspaceRoot, "specs", "008-api-dto-logging", "contracts", "api-dto-logging.openapi.yaml");

        File.Exists(canonicalContract).ShouldBeTrue();
        File.Exists(mirroredContract).ShouldBeTrue();

        var canonicalContent = NormalizeLineEndings(File.ReadAllText(canonicalContract));
        var mirroredContent = NormalizeLineEndings(File.ReadAllText(mirroredContract));

        canonicalContent.ShouldBe(mirroredContent);
        canonicalContent.ShouldContain("/management-users");
        canonicalContent.ShouldContain("/management-users/login");
        canonicalContent.ShouldNotContain("/management-users/{id}");
        canonicalContent.ShouldContain("/management-tasks");
        canonicalContent.ShouldNotContain("sortBy");
        canonicalContent.ShouldNotContain("sortDirection");
        canonicalContent.ShouldContain("BearerAuth");
        canonicalContent.ShouldContain("ManagementTaskResponseDto");
        canonicalContent.ShouldContain("ManagementTaskSummaryResponseDto");
        canonicalContent.ShouldContain("ManagementUserResponseDto");
        canonicalContent.ShouldContain("ManagementUserLoginResponseDto");
        canonicalContent.ShouldContain("ManagementTaskStatus:");
        canonicalContent.ShouldContain("- Pending");
        canonicalContent.ShouldContain("- InProgress");
        canonicalContent.ShouldContain("- Completed");
    }

    private static string NormalizeLineEndings(string content)
    {
        return content.Replace("\r\n", "\n");
    }
}
