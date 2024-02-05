using CliWrap;
using CliWrap.Buffered;

namespace PmtScaffolder;

public static class PSCmd
{
  public static async Task<BufferedCommandResult> RunPowerShell(string path, string cmd)
  {
    try
    {
      return await Cli.Wrap("powershell").WithWorkingDirectory(path).WithArguments(cmd).ExecuteBufferedAsync();
    }
    catch (Exception ex)
    {
      Console.WriteLine("\n\n=============== Project path may be incorrect ===============\n\n");
      Console.WriteLine(ex.ToString() + '\n');
      return null;
    }
  }
}
