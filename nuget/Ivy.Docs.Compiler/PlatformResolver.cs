using System;

namespace Ivy.Docs.Compiler;

public static class PlatformResolver
{
    private static readonly string[] LinuxDistros =
        ["ubuntu", "debian", "fedora", "centos", "rhel", "alpine", "opensuse", "sles", "ol", "linuxmint", "tizen"];

    private static readonly string[] MacOsDistros = ["macos", "osx."];

    public static string ResolvePlatform(string rid)
    {
        rid = NormalizeRid(rid);
        return rid switch
        {
            _ when rid.StartsWith("win") && rid.Contains("arm64") => "win-arm64",
            _ when rid.StartsWith("win") => "win-x64",
            _ when rid.StartsWith("linux") && rid.Contains("arm64") => "linux-arm64",
            _ when rid.StartsWith("linux") => "linux-x64",
            _ when rid.StartsWith("osx") && rid.Contains("arm64") => "osx-arm64",
            _ when rid.StartsWith("osx") => "osx-x64",
            _ => throw new PlatformNotSupportedException($"Unsupported platform: {rid}")
        };
    }

    private static string NormalizeRid(string rid)
    {
        foreach (var distro in LinuxDistros)
        {
            if (rid.StartsWith(distro, StringComparison.OrdinalIgnoreCase))
            {
                var archIndex = rid.LastIndexOf('-');
                var arch = archIndex >= 0 ? rid[archIndex..] : "-x64";
                return "linux" + arch;
            }
        }
        foreach (var distro in MacOsDistros)
        {
            if (rid.StartsWith(distro, StringComparison.OrdinalIgnoreCase))
            {
                var archIndex = rid.LastIndexOf('-');
                var arch = archIndex >= 0 ? rid[archIndex..] : "-x64";
                return "osx" + arch;
            }
        }
        return rid;
    }

    public static string GetBinaryName(bool isWindows) =>
        isWindows ? "ivy-docs-cli.exe" : "ivy-docs-cli";

    public static string GetNativePath(string assemblyDir, string platform, string binaryName) =>
        Path.Combine(assemblyDir, "runtimes", platform, "native", binaryName);
}
