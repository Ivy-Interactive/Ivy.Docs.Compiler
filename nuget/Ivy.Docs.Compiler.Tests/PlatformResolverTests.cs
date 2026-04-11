using System.Runtime.InteropServices;
using Xunit;
using Ivy.Docs.Compiler;

namespace Ivy.Docs.Compiler.Tests;

public class PlatformResolverTests
{
    [Theory]
    [InlineData("win-x64", "win-x64")]
    [InlineData("win-arm64", "win-arm64")]
    [InlineData("win10-x64", "win-x64")]
    [InlineData("linux-x64", "linux-x64")]
    [InlineData("linux-arm64", "linux-arm64")]
    [InlineData("osx-x64", "osx-x64")]
    [InlineData("osx-arm64", "osx-arm64")]
    public void ResolvePlatform_ReturnsExpectedPlatform(string rid, string expected)
    {
        var result = PlatformResolver.ResolvePlatform(rid);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("ubuntu.22.04-x64", "linux-x64")]
    [InlineData("ubuntu.22.04-arm64", "linux-arm64")]
    [InlineData("debian.12-arm64", "linux-arm64")]
    [InlineData("debian.11-x64", "linux-x64")]
    [InlineData("fedora.39-x64", "linux-x64")]
    [InlineData("fedora.39-arm64", "linux-arm64")]
    [InlineData("alpine.3.18-x64", "linux-x64")]
    [InlineData("alpine.3.18-arm64", "linux-arm64")]
    [InlineData("rhel.9-x64", "linux-x64")]
    [InlineData("centos.7-x64", "linux-x64")]
    [InlineData("Ubuntu.22.04-x64", "linux-x64")]
    [InlineData("Debian.12-arm64", "linux-arm64")]
    [InlineData("RHEL.9-x64", "linux-x64")]
    public void ResolvePlatform_DistroSpecificRid_ReturnsLinuxPlatform(string rid, string expected)
    {
        var result = PlatformResolver.ResolvePlatform(rid);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ResolvePlatform_UnknownRid_Throws()
    {
        Assert.Throws<PlatformNotSupportedException>(
            () => PlatformResolver.ResolvePlatform("freebsd-x64"));
    }

    [Theory]
    [InlineData("ubuntu.22.04-x64", "x64")]
    [InlineData("ubuntu.24.04-arm64", "arm64")]
    [InlineData("debian.11-x64", "x64")]
    [InlineData("debian.12-arm64", "arm64")]
    [InlineData("alpine.3.17-x64", "x64")]
    [InlineData("rhel.8-x64", "x64")]
    [InlineData("fedora.38-arm64", "arm64")]
    [InlineData("centos.7-x64", "x64")]
    public void ResolvePlatform_DistroSpecificRid_MapsToNormalizedPlatform(string rid, string arch)
    {
        // Distro-specific RIDs don't start with linux/win/osx, so they hit the
        // RuntimeInformation.IsOSPlatform() fallback. The expected result depends
        // on the host OS where the test is running.
        string expectedPrefix;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            expectedPrefix = "linux";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            expectedPrefix = "osx";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            expectedPrefix = "win";
        else
            throw new PlatformNotSupportedException("Unknown host OS for test");

        var expected = $"{expectedPrefix}-{arch}";
        var result = PlatformResolver.ResolvePlatform(rid);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetBinaryName_Windows_ReturnsExe()
    {
        Assert.Equal("ivy-docs-cli.exe", PlatformResolver.GetBinaryName(true));
    }

    [Fact]
    public void GetBinaryName_Unix_ReturnsNoExtension()
    {
        Assert.Equal("ivy-docs-cli", PlatformResolver.GetBinaryName(false));
    }

    [Fact]
    public void GetNativePath_CombinesCorrectly()
    {
        var result = PlatformResolver.GetNativePath("dir", "win-x64", "ivy-docs-cli.exe");
        var expected = Path.Combine("dir", "runtimes", "win-x64", "native", "ivy-docs-cli.exe");
        Assert.Equal(expected, result);
    }
}
