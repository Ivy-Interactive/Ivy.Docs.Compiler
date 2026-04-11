using Xunit;

namespace Ivy.Docs.Compiler.Tests;

public class PlatformResolverTests
{
    [Theory]
    [InlineData("win-x64", "win-x64")]
    [InlineData("win10-x64", "win-x64")]
    public void ResolvePlatform_WindowsX64_ReturnsWinX64(string rid, string expected)
    {
        Assert.Equal(expected, PlatformResolver.ResolvePlatform(rid));
    }

    [Theory]
    [InlineData("win-arm64", "win-arm64")]
    [InlineData("win10-arm64", "win-arm64")]
    public void ResolvePlatform_WindowsArm64_ReturnsWinArm64(string rid, string expected)
    {
        Assert.Equal(expected, PlatformResolver.ResolvePlatform(rid));
    }

    [Theory]
    [InlineData("linux-x64", "linux-x64")]
    public void ResolvePlatform_LinuxX64_ReturnsLinuxX64(string rid, string expected)
    {
        Assert.Equal(expected, PlatformResolver.ResolvePlatform(rid));
    }

    [Theory]
    [InlineData("linux-arm64", "linux-arm64")]
    public void ResolvePlatform_LinuxArm64_ReturnsLinuxArm64(string rid, string expected)
    {
        Assert.Equal(expected, PlatformResolver.ResolvePlatform(rid));
    }

    [Theory]
    [InlineData("osx-x64", "osx-x64")]
    public void ResolvePlatform_OsxX64_ReturnsOsxX64(string rid, string expected)
    {
        Assert.Equal(expected, PlatformResolver.ResolvePlatform(rid));
    }

    [Theory]
    [InlineData("osx-arm64", "osx-arm64")]
    [InlineData("osx.15-arm64", "osx-arm64")]
    public void ResolvePlatform_OsxArm64_ReturnsOsxArm64(string rid, string expected)
    {
        Assert.Equal(expected, PlatformResolver.ResolvePlatform(rid));
    }

    [Fact]
    public void ResolvePlatform_UnknownRid_Throws()
    {
        Assert.Throws<PlatformNotSupportedException>(
            () => PlatformResolver.ResolvePlatform("freebsd-x64"));
    }

    [Fact]
    public void GetBinaryName_Windows_HasExeExtension()
    {
        Assert.Equal("ivy-docs-cli.exe", PlatformResolver.GetBinaryName(true));
    }

    [Fact]
    public void GetBinaryName_Unix_NoExtension()
    {
        Assert.Equal("ivy-docs-cli", PlatformResolver.GetBinaryName(false));
    }

    [Fact]
    public void GetNativePath_BuildsCorrectPath()
    {
        var result = PlatformResolver.GetNativePath("/app", "osx-arm64", "ivy-docs-cli");
        var expected = Path.Combine("/app", "runtimes", "osx-arm64", "native", "ivy-docs-cli");
        Assert.Equal(expected, result);
    }
}
