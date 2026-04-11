using System.Diagnostics;
using System.Runtime.InteropServices;
using Ivy.Docs.Compiler;

var rid = RuntimeInformation.RuntimeIdentifier;

var platform = PlatformResolver.ResolvePlatform(rid);

var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
var binaryName = PlatformResolver.GetBinaryName(isWindows);

// Look for the native binary relative to the executing assembly
var assemblyDir = Path.GetDirectoryName(typeof(Program).Assembly.Location)
    ?? AppContext.BaseDirectory;

var nativePath = PlatformResolver.GetNativePath(assemblyDir, platform, binaryName);

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
