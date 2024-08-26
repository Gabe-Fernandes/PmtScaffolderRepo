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

    Validation.PrintErrorReport();
    _userInput.ErrorReport = [];
    _userInput.OverWrite = string.Empty;
  }

  private static async Task<string> GenerateCode(string filePath, string fileType)
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

      // Generate code files
      for (int j = 0; j < _userInput.FileNames[i].Count; j++)
      {
        string file = _userInput.FileNames[i][j];
        switch (fileType)
        {
          case "scss": await PSCmd.RunPowerShellBatch(currentControllerPath, FrontEndTemplates.SassFile(file), file + ".scss");
                       await Util.InsertWithCheck(FrontEndTemplates.SassProjFileCmd(controller, file), _userInput.ProjPath, $"{_userInput.ProjName}.csproj", insertAtTop: false, htmlLandmark: true); break;
          case "js": await PSCmd.RunPowerShellBatch(currentControllerPath, FrontEndTemplates.JsFile(file), file + ".js"); break;
          case "cshtml": await PSCmd.RunPowerShellBatch(currentControllerPath, FrontEndTemplates.CsHtmlFile(file, controller), file + ".cshtml");
                         await Util.InsertWithCheck(FrontEndTemplates.CssLinkEle(file), currentControllerPath, $"_{controller}_Layout.cshtml", insertAtTop: false, htmlLandmark: true);
                         await Util.InsertCode(_userInput.ProjPath + "/Controllers", FrontEndTemplates.ControllerGetMethod(file), $"{controller}Controller.cs"); break;
        }
      }
    }

    return $"{fileType} scaffold complete";
  }

  private static async Task<string> GenerateControllerClass(string filePath)
  {
    switch (Util.TestPath(_userInput.ProjPath, filePath))
    {
      case 0: await PSCmd.RunPowerShell(_userInput.ProjPath, $"mkdir {filePath}"); break;
      case 1: break;
      default: return $"Failed to test path: {filePath}";
    }

    for (int i = 0; i < _userInput.Controllers.Count; i++)
    {
      await PSCmd.RunPowerShellBatch(filePath, FrontEndTemplates.ControllerFile(_userInput.Controllers[i]), $"{_userInput.Controllers[i]}Controller.cs");
    }

    return "controller class(es) complete";
  }

  private static async Task GenerateControllerDir(string filePath, int i, string fileType)
  {
    await PSCmd.RunPowerShell(filePath, $"mkdir {Util.Capital(_userInput.Controllers[i], true)}");

    if (fileType == "cshtml")
    {
      string currentControllerPath = $"{filePath}/{_userInput.Controllers[i]}";
      await PSCmd.RunPowerShellBatch(currentControllerPath, FrontEndTemplates.ViewStartFile(_userInput.Controllers[i]), "_ViewStart.cshtml");
      await PSCmd.RunPowerShellBatch(currentControllerPath, FrontEndTemplates.LayoutFile(_userInput.Controllers[i]), $"_{_userInput.Controllers[i]}_Layout.cshtml");
    }
  }
}
