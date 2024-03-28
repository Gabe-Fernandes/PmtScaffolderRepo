using CliWrap;
using CliWrap.Buffered;

namespace PmtScaffolder;

public static class PSCmd
{
  private static readonly UserInput _userInput = UserInput.GetUserInput();

  public static async Task<BufferedCommandResult> RunPowerShell(string path, string cmd)
  {
    try
    {
      Console.WriteLine($"Cmd: {cmd}");
      return await Cli.Wrap("powershell").WithWorkingDirectory(path).WithArguments(cmd).ExecuteBufferedAsync();
    }
    catch (Exception ex)
    {
      HandleException(ex, path, cmd);
      return null;
    }
  }

  public static async Task<BufferedCommandResult> RunPowerShellBatch(string path, string[] cmd, string fileNameWithExtension, bool isInsertion = false)
  {
    // warn about overwriting if it's not an insertion
    if (isInsertion == false)
    {
      switch (Util.TestPath(path, Path.Combine(path, fileNameWithExtension)))
      {
        case 0: break;
        case 1:
          if (_userInput.OverWrite == "false") { return null; }
          if (_userInput.OverWrite == "true") { break; }

          bool validInput = false;
          while (validInput == false)
          {
            Console.WriteLine($"\n\n ================== Scaffolding Paused ================== \n\n");
            Console.WriteLine($"The file {fileNameWithExtension} already exists at {path}. Overwrite the existing file?");
            Console.WriteLine("[Y] Yes  [A] Yes to All  [N] No  [L] No to All");
            Console.Write("Response: ");
            string input = Console.ReadLine();
            Console.WriteLine($"\n\n\n\n");
            validInput = true;
            switch (input.ToUpper())
            {
              case "Y": break;
              case "A": _userInput.OverWrite = "true"; break;
              case "N": return null;
              case "L": _userInput.OverWrite = "false"; return null;
              default: validInput = false; break;
            }
          }
          break;
        default: return null;
      }
    }

    try
    {
      for (int i = 0; i < cmd.Length; i++)
      {
        Console.WriteLine($"Cmd: {cmd[i]}");
      }

      return await Cli.Wrap("powershell").WithWorkingDirectory(path).WithArguments(cmd).ExecuteBufferedAsync();
    }
    catch (Exception ex)
    {
      HandleException(ex, path, cmd[0]);
      return null;
    }
  }

  private static void HandleException(Exception ex, string path, string cmd)
  {
    _userInput.ErrorReport.Add("Project path may be incorrect");
    _userInput.ErrorReport.Add("Path:");
    _userInput.ErrorReport.Add(path);
    _userInput.ErrorReport.Add("Command:");
    _userInput.ErrorReport.Add(cmd);
    _userInput.ErrorReport.Add("Exception:");
    _userInput.ErrorReport.Add(ex.ToString() + "\n\n");
  }
}
