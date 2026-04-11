using System.Runtime.InteropServices;

namespace Ivy.Docs.Compiler;

public static class PlatformResolver
{
    public static string ResolvePlatform(string rid) => rid switch
    {
        _ when rid.StartsWith("win") && rid.Contains("arm64") => "win-arm64",
        _ when rid.StartsWith("win") => "win-x64",
        _ when rid.StartsWith("linux") && rid.Contains("arm64") => "linux-arm64",
        _ when rid.StartsWith("linux") => "linux-x64",
        _ when rid.StartsWith("osx") && rid.Contains("arm64") => "osx-arm64",
        _ when rid.StartsWith("osx") => "osx-x64",
        _ => throw new PlatformNotSupportedException($"Unsupported platform: {rid}")
    };

    public static string GetBinaryName(bool isWindows) =>
        isWindows ? "ivy-docs-cli.exe" : "ivy-docs-cli";

    public static string GetNativePath(string assemblyDir, string platform, string binaryName) =>
        Path.Combine(assemblyDir, "runtimes", platform, "native", binaryName);
}
