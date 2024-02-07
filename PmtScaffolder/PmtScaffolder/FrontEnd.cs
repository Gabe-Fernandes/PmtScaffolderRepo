namespace PmtScaffolder;

public static class FrontEnd
{
  private static readonly UserInput _userInput = UserInput.GetUserInput();



  public static async Task ScaffoldFrontEndCode()
  {
    Console.WriteLine(await GenerateControllerClass(_userInput.ProjPath + "/Controllers"));
    Console.WriteLine(await GenerateCode(_userInput.ProjPath + "/Styles", "scss"));
    Console.WriteLine(await GenerateCode(_userInput.ProjPath + "/wwwroot/js", "js"));
    Console.WriteLine(await GenerateCode(_userInput.ProjPath + "/Views", "cshtml"));
    // overwrite needs to check if files exist
    // pmt help
  }

  private static async Task<string> GenerateCode(string filePath, string fileType, bool overwrite = true)
  {
    switch (Util.TestPath(_userInput.ProjPath, filePath))
    {
      case 0: await PSCmd.RunPowerShell(_userInput.ProjPath, $"mkdir {filePath}"); break;
      case 1: break;
      default: return $"Failed to test path: {filePath}";
    }

    for (int i = 0; i < _userInput.Controllers.Count; i++)
    {
      string currentControllerPath = $"{filePath}/{_userInput.Controllers[i]}";
      string controller = _userInput.Controllers[i];
      // Ensure there is a dir for each controller
      switch (Util.TestPath(filePath, currentControllerPath))
      {
        case 0: await GenerateControllerDir(filePath, i, fileType); break;
        case 1: break;
        default: return $"Failed to test path: {currentControllerPath}";
      }
      
      if (overwrite == false) { continue; }

      // Generate code files
      for (int j = 0; j < _userInput.FileNames[i].Count; j++)
      {
        string file = _userInput.FileNames[i][j];
        switch (fileType)
        {
          case "scss": await PSCmd.RunPowerShellBatch(currentControllerPath, FrontEndTemplates.SassFile(file)); break;
          case "js": await PSCmd.RunPowerShellBatch(currentControllerPath, FrontEndTemplates.JsFile(file)); break;
          case "cshtml": await PSCmd.RunPowerShellBatch(currentControllerPath, FrontEndTemplates.CsHtmlFile(file, controller));
                         await Util.InsertCode(currentControllerPath, FrontEndTemplates.CssLinkEle(file), $"_{controller}_Layout.cshtml", true);
                         await Util.InsertCode(_userInput.ProjPath + "/Controllers", FrontEndTemplates.ControllerGetMethod(file), $"{controller}Controller.cs"); break;
        }
      }
    }

    return $"{fileType} scaffold complete";
  }

  private static async Task<string> GenerateControllerClass(string filePath, bool overwrite = true)
  {
    for (int i = 0; i < _userInput.Controllers.Count; i++)
    {
      if (overwrite == false) { return string.Empty; }
      await PSCmd.RunPowerShellBatch(filePath, FrontEndTemplates.ControllerFile(_userInput.Controllers[i]));
    }

    return "controller class(es) complete";
  }

  private static async Task GenerateControllerDir(string filePath, int i, string fileType)
  {
    await PSCmd.RunPowerShell(filePath, $"mkdir {_userInput.Controllers[i]}");

    if (fileType == "cshtml")
    {
      string currentControllerPath = $"{filePath}/{_userInput.Controllers[i]}";
      await PSCmd.RunPowerShellBatch(currentControllerPath, FrontEndTemplates.ViewStartFile(_userInput.Controllers[i]));
      await PSCmd.RunPowerShellBatch(currentControllerPath, FrontEndTemplates.LayoutFile(_userInput.Controllers[i]));
    }
  }
}
