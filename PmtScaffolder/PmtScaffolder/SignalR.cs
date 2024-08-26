namespace PmtScaffolder;

public static class SignalR
{
  private static readonly string br = Environment.NewLine;
  private static readonly UserInput _userInput = UserInput.GetUserInput();

  public static async Task ScaffoldSignalRCode()
  {
    // ensure SignalR folder exists
    if (Util.TestPath(_userInput.ProjPath, _userInput.ProjPath + "/SignalRHubs") == 0)
    {
      await PSCmd.RunPowerShell(_userInput.ProjPath, $"mkdir SignalRHubs");
    }

    // add signalR service to DI container
    await Util.InsertWithCheck(SignalRTemplates.AddSignalRService(), _userInput.ProjPath, "program.cs", false, "// SignalR Landmark");

    await GenerateCode();

    Validation.PrintErrorReport();
    _userInput.ErrorReport = [];
    _userInput.OverWrite = string.Empty;
  }

  private static async Task GenerateCode()
  {
    string signalRHubsPath = Path.Combine(_userInput.ProjPath, "SignalRHubs");

    for (int i = 0; i < _userInput.Controllers.Count; i++)
    {
      // ensure there is a dir for the controller name
      string controllerName = Util.Capital(_userInput.Controllers[i], true);
      if (Util.TestPath(signalRHubsPath, signalRHubsPath + $"/{controllerName}Hubs") == 0)
      {
        await PSCmd.RunPowerShell(signalRHubsPath, $"mkdir {controllerName}Hubs");
      }

      for (int j = 0; j < _userInput.FileNames[i].Count; j++)
      {
        // ensure there is a dir for the file name
        string fileName = Util.Capital(_userInput.FileNames[i][j], true);
        string ControllerHubPath = Path.Combine(signalRHubsPath, $"{controllerName}Hubs");
        if (Util.TestPath(ControllerHubPath, ControllerHubPath + $"/{fileName}Files") == 0)
        {
          await PSCmd.RunPowerShell(ControllerHubPath, $"mkdir {fileName}Files");
        }

        // map hubs in the middleware pipeline
        await Util.InsertWithCheck(SignalRTemplates.MapHubMiddleWare(fileName), _userInput.ProjPath, "program.cs", false, "// Map Hubs Landmark");
        // insert using directives for each hub mapping
        await Util.InsertWithCheck($"using {_userInput.ProjName}.SignalRHubs.{controllerName}Hubs.{fileName}Files;{br}", _userInput.ProjPath, "program.cs", true);

        string hubFilesPath = Path.Combine(ControllerHubPath, $"{fileName}Files");
        await PSCmd.RunPowerShellBatch(hubFilesPath, SignalRTemplates.HubDTO(controllerName, fileName), $"{fileName}DTOs.cs");
        await PSCmd.RunPowerShellBatch(hubFilesPath, SignalRTemplates.HubInterface(controllerName, fileName), $"I{fileName}Hub.cs");
        await PSCmd.RunPowerShellBatch(hubFilesPath, SignalRTemplates.ServerSideHub(controllerName, fileName), $"{fileName}Hub.cs");

        string jsFileName = Util.Capital(fileName, false);
        await PSCmd.RunPowerShellBatch($"{_userInput.ProjPath}/wwwroot/js/{controllerName}", SignalRTemplates.ClientSideHub(jsFileName), jsFileName + ".js");
      }
    }
    Console.WriteLine("SignalR scaffold complete");
  }
}
