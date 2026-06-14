using Shouldly;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class OpenApiContractTests
{
    [Fact]
    public void CanonicalAndMirroredContracts_ShouldStayInSync()
    {
        var repositoryRoot = TestPathHelper.GetRepositoryRoot();
        var canonicalContract = Path.Combine(repositoryRoot, "openapi", "openapi.yaml");
        var mirroredContract = Path.Combine(repositoryRoot, "specs", "005-api-project", "contracts", "management-tasks.openapi.yaml");

        File.Exists(canonicalContract).ShouldBeTrue();
        File.Exists(mirroredContract).ShouldBeTrue();

        var canonicalContent = NormalizeLineEndings(File.ReadAllText(canonicalContract));
        var mirroredContent = NormalizeLineEndings(File.ReadAllText(mirroredContract));

        canonicalContent.ShouldBe(mirroredContent);
        canonicalContent.ShouldContain("/management-tasks");
        canonicalContent.ShouldContain("CreateManagementTaskRequestDto");
        canonicalContent.ShouldContain("ManagementTaskResponseDto");
    }

    private static string NormalizeLineEndings(string content)
    {
        return content.Replace("\r\n", "\n");
    }
}
