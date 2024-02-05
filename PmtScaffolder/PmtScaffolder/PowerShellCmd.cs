using CliWrap;
using CliWrap.Buffered;

namespace PmtScaffolder;

public static class PowerShellCmd
{
  public static async Task<string> RunPowerShell(string path, string cmd)
  {
    var powerShellResults = await Cli.Wrap("powershell").WithWorkingDirectory(path).WithArguments(cmd).ExecuteBufferedAsync();

    return powerShellResults.StandardError;
  }
}
