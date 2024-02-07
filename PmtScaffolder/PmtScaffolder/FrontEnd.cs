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
    // (insertion) controller get methods
    // (insertion) viewstart CSS
    // overwrite needs to check if files exist
    //await InsertCode(_userInput.ProjPath, "\n// hello 1", "program.cs");
  }

  private static async Task<string> GenerateCode(string filePath, string fileType, bool overwrite = true)
  {
    switch (TestPath(_userInput.ProjPath, filePath))
    {
      case 0: await PSCmd.RunPowerShell(_userInput.ProjPath, $"mkdir {filePath}"); break;
      case 1: break;
      default: return $"Failed to test path: {filePath}";
    }

    for (int i = 0; i < _userInput.Controllers.Count; i++)
    {
      string currentControllerPath = $"{filePath}/{_userInput.Controllers[i]}";
      // Ensure there is a dir for each controller
      switch (TestPath(filePath, currentControllerPath))
      {
        case 0: await GenerateControllerDir(filePath, i, fileType); break;
        case 1: break;
        default: return $"Failed to test path: {currentControllerPath}";
      }
      
      if (overwrite == false) { continue; }

      // Generate code files
      for (int j = 0; j < _userInput.FileNames[i].Count; j++)
      {
        switch (fileType)
        {
          case "scss": await PSCmd.RunPowerShellBatch(currentControllerPath, FrontEndTemplates.SassFile(_userInput.FileNames[i][j])); break;
          case "js": await PSCmd.RunPowerShellBatch(currentControllerPath, FrontEndTemplates.JsFile(_userInput.FileNames[i][j])); break;
          case "cshtml": await PSCmd.RunPowerShellBatch(currentControllerPath, FrontEndTemplates.CsHtmlFile(_userInput.FileNames[i][j], _userInput.Controllers[i]));
                         await InsertCode(currentControllerPath, FrontEndTemplates.CssLinkEle(_userInput.FileNames[i][j]), $"_{_userInput.Controllers[i]}_Layout.cshtml"); break;
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

  private static int TestPath(string currentPathToTestFrom, string pathToTest)
  {
    var bufferedCmd = PSCmd.RunPowerShell(currentPathToTestFrom, $"Test-Path -Path {pathToTest}");
    if (bufferedCmd.Result == null) { return -1; }
    return (bufferedCmd.Result.StandardOutput == "True\r\n") ? 1 : 0;
  }

  private static async Task InsertCode(string parentPath, string codeToInsert, string fileNameWithExtension) // works, but adds empty lines at the end of the file and changes indenting
  {
    // extract file text
    string pathToFile = Path.Combine(parentPath, fileNameWithExtension);
    var fileContent = await PSCmd.RunPowerShell(parentPath, $"Get-Content -Path {pathToFile} -Raw");
    if (fileContent == null) { return; }
    string fileText = fileContent.StandardOutput;

    // locate PMT Landmark
    const string landmark = "// PMT Landmark";
    int landmarkIndex = fileText.IndexOf(landmark);
    if (landmarkIndex == -1)
    {
      Console.WriteLine($"\n===========================Please ensure the code file at {pathToFile} has the comment, \"{landmark}\" above where lines of code are to be inserted.===========================\n");
      return;
    }

    // partition code file, concatenate, and write
    string preLandmark = fileText.Substring(0, landmarkIndex + landmark.Length);
    string postLandmark = fileText.Substring(landmarkIndex + landmark.Length); // +1?
    string finalOutput = preLandmark + codeToInsert + postLandmark;
    finalOutput = finalOutput.Replace("\"", "\\\"");
    finalOutput = finalOutput.Replace("  ", "\t");
    finalOutput = finalOutput.Substring(0, finalOutput.Length - 4); // chop the last 2 line breaks off
    await PSCmd.RunPowerShell(parentPath, $"Write-Output '{finalOutput}' > {fileNameWithExtension}");
  }
}
