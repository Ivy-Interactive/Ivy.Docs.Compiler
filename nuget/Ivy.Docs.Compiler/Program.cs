using System.Diagnostics;
using System.Runtime.InteropServices;

var rid = RuntimeInformation.RuntimeIdentifier;

// Normalize RID to one of the supported platforms
var platform = rid switch
{
    _ when rid.StartsWith("win") && rid.Contains("arm64") => "win-arm64",
    _ when rid.StartsWith("win") => "win-x64",
    _ when rid.StartsWith("linux") && rid.Contains("arm64") => "linux-arm64",
    _ when rid.StartsWith("linux") => "linux-x64",
    _ when rid.StartsWith("osx") && rid.Contains("arm64") => "osx-arm64",
    _ when rid.StartsWith("osx") => "osx-x64",
    // Fallback for distro-specific RIDs (e.g. ubuntu.22.04-x64, debian.11-arm64)
    _ when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && rid.Contains("arm64") => "linux-arm64",
    _ when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => "linux-x64",
    _ when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && rid.Contains("arm64") => "osx-arm64",
    _ when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => "osx-x64",
    _ when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && rid.Contains("arm64") => "win-arm64",
    _ when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => "win-x64",
    _ => throw new PlatformNotSupportedException($"Unsupported platform: {rid}")
};

var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
var binaryName = isWindows ? "ivy-docs-cli.exe" : "ivy-docs-cli";

// Look for the native binary relative to the executing assembly
var assemblyDir = Path.GetDirectoryName(typeof(Program).Assembly.Location)
    ?? AppContext.BaseDirectory;

var nativePath = Path.Combine(assemblyDir, "runtimes", platform, "native", binaryName);

if (!File.Exists(nativePath))
{
    Console.Error.WriteLine($"Native binary not found at: {nativePath}");
    Console.Error.WriteLine($"Detected RID: {rid}, mapped platform: {platform}");
    return 1;
}

// Ensure executable permission on Unix
if (!isWindows)
{
    File.SetUnixFileMode(nativePath,
        UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
        UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
        UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
}

var psi = new ProcessStartInfo
{
    FileName = nativePath,
    UseShellExecute = false,
};

foreach (var arg in args)
{
    psi.ArgumentList.Add(arg);
}

using var process = Process.Start(psi);
if (process == null)
{
    Console.Error.WriteLine("Failed to start native process");
    return 1;
}

await process.WaitForExitAsync();
return process.ExitCode;
