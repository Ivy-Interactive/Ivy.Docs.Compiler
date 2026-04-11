using Xunit;
using Ivy.Docs.Compiler;

namespace Ivy.Docs.Compiler.Tests;

public class PlatformResolverTests
{
    [Theory]
    [InlineData("win-x64", "win-x64")]
    [InlineData("win10-x64", "win-x64")]
    public void ResolvePlatform_WinX64(string rid, string expected)
    {
        Assert.Equal(expected, PlatformResolver.ResolvePlatform(rid));
    }

    [Fact]
    public void ResolvePlatform_WinArm64()
    {
        Assert.Equal("win-arm64", PlatformResolver.ResolvePlatform("win-arm64"));
    }

    [Fact]
    public void ResolvePlatform_LinuxX64()
    {
        Assert.Equal("linux-x64", PlatformResolver.ResolvePlatform("linux-x64"));
    }

    [Fact]
    public void ResolvePlatform_LinuxArm64()
    {
        Assert.Equal("linux-arm64", PlatformResolver.ResolvePlatform("linux-arm64"));
    }

    [Fact]
    public void ResolvePlatform_OsxX64()
    {
        Assert.Equal("osx-x64", PlatformResolver.ResolvePlatform("osx-x64"));
    }

    [Fact]
    public void ResolvePlatform_OsxArm64()
    {
        Assert.Equal("osx-arm64", PlatformResolver.ResolvePlatform("osx-arm64"));
    }

    [Fact]
    public void ResolvePlatform_UnknownRid_Throws()
    {
        Assert.Throws<PlatformNotSupportedException>(
            () => PlatformResolver.ResolvePlatform("freebsd-x64"));
    }

    [Fact]
    public void GetBinaryName_Windows()
    {
        Assert.Equal("ivy-docs-cli.exe", PlatformResolver.GetBinaryName(true));
    }

    [Fact]
    public void GetBinaryName_Unix()
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
