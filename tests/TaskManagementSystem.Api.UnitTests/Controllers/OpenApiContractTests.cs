using Shouldly;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class OpenApiContractTests
{
    [Fact]
    public void CanonicalAndMirroredContracts_ShouldStayInSync()
    {
        var repositoryRoot = TestPathHelper.GetRepositoryRoot();
        var canonicalContract = Path.Combine(repositoryRoot, "openapi", "openapi.yaml");
        var mirroredContract = Path.Combine(repositoryRoot, "specs", "007-management-user-auth", "contracts", "management-users-auth.openapi.yaml");

        File.Exists(canonicalContract).ShouldBeTrue();
        File.Exists(mirroredContract).ShouldBeTrue();

        var canonicalContent = NormalizeLineEndings(File.ReadAllText(canonicalContract));
        var mirroredContent = NormalizeLineEndings(File.ReadAllText(mirroredContract));

        canonicalContent.ShouldBe(mirroredContent);
        canonicalContent.ShouldContain("/management-users");
        canonicalContent.ShouldContain("/management-users/login");
        canonicalContent.ShouldContain("/management-tasks");
        canonicalContent.ShouldContain("BearerAuth");
    }

    private static string NormalizeLineEndings(string content)
    {
        return content.Replace("\r\n", "\n");
    }
}
