namespace PmtScaffolder;

public static class FrontEnd
{
  private static readonly UserInput _userInput = UserInput.GetUserInput();

  public static async Task ScaffoldCode()
  {
    string stylesPath = _userInput.ProjPath + "/Styles";

    switch (TestPath(stylesPath))
    {
      case 0: await PSCmd.RunPowerShell(_userInput.ProjPath, "mkdir Styles"); break;
      case 1: break;
      default: return;
    }

    for (int i = 0; i < _userInput.Controllers.Count; i++)
    {
      // Test path for controller Dir
      for (int j = 0; j < _userInput.FileNames.Count; j++)
      {

      }
    }
  }

  private static int TestPath(string pathToTest)
  {
    var bufferedCmd = PSCmd.RunPowerShell(_userInput.ProjPath, $"Test-Path -Path {pathToTest}");
    if (bufferedCmd.Result == null) { return -1; }
    return (bufferedCmd.Result.StandardOutput == "True\r\n") ? 1 : 0;
  }
}
