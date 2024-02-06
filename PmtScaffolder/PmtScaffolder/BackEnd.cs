namespace PmtScaffolder;

public static class BackEnd
{
  private static readonly UserInput _userInput = UserInput.GetUserInput();
  private static readonly string br = Environment.NewLine;



  public static async Task ScaffoldBackEndCode()
  {
    Console.WriteLine(await GenerateCode(_userInput.ProjPath + "/Data/Models", "model class"));
    Console.WriteLine(await GenerateCode(_userInput.ProjPath + "/Data/RepoInterfaces", "repo interface"));
    Console.WriteLine(await GenerateCode(_userInput.ProjPath + "/Data/Repos", "repository"));
    Console.WriteLine(await GenerateCode(_userInput.TestProjPath + "/Data/Repos", "unit test"));
    // (insertion) add transient Repo services to the container for DI in program.cs
    // write unit tests for controllers
    // create test project (consider a 3rd area - front, back, tests)
    // (insertion) add db sets to dbCtx
    // create dbctx
    // AppUser edge case with unit tests
  }

  private static async Task<string> GenerateCode(string filePath, string fileType, bool overwrite = false)
  {
    switch (TestPath(_userInput.ProjPath, filePath))
    {
      case 0: await PSCmd.RunPowerShell(_userInput.ProjPath, $"mkdir {filePath}"); break;
      case 1: break;
      default: return $"Failed to test path: {filePath}";
    }

    if (overwrite == false) { return string.Empty; }

    for (int i = 0; i < _userInput.Models.Count; i++)
    {
      List<string> props = [];
      List<string> mockData = [];
      List<string> dbCtxMockData = [];

      for (int j = 0; j < _userInput.Properties[i].Count; j++)
      {
        props.Add($"{br}\tpublic {_userInput.DataTypes[i][j]} {_userInput.Properties[i][j]} {{ get; set; }}");
        mockData.Add($"{br}\t\t{_userInput.Properties[i][j]} = {GetUnitTestMockData(_userInput.DataTypes[i][j])},");
        if (_userInput.Properties[i][j] != "Id") // Id is hardcoded in the template
        {
          dbCtxMockData.Add($"{br}\t\t\t\t\t{_userInput.Properties[i][j]} = {GetUnitTestMockData(_userInput.DataTypes[i][j])},");
        }
      }
      props.Add($"{br}}}'");
      props.Add($"> {_userInput.Models[i]}.cs");

      switch (fileType)
      {
        case "model class": await PSCmd.RunPowerShellBatch(filePath, BackEndTemplates.ModelClassHeader(_userInput.Models[i]).Concat(props).ToArray()); break;
        case "repo interface": await PSCmd.RunPowerShellBatch(filePath, BackEndTemplates.RepoInterface(_userInput.Models[i])); break;
        case "repository": await PSCmd.RunPowerShellBatch(filePath, BackEndTemplates.Repository(_userInput.Models[i])); break;
        case "unit test": await PSCmd.RunPowerShellBatch(filePath, BackEndTemplates.UnitTest(_userInput.Models[i], mockData.ToArray(), dbCtxMockData.ToArray())); break;
      }
    }

    return $"{fileType} scaffold complete";
  }

  private static string GetUnitTestMockData(string dataType)
  {
    return dataType switch
    {
      "string" => "\"test string\"",
      "int" => "2",
      "float" => "2.5",
      "DateTime" => "DateTime.Now",
      _ => "ERROR",
    };
  }

  private static int TestPath(string currentPathToTestFrom, string pathToTest) // consider putting a single TestPath method somewhere
  {
    var bufferedCmd = PSCmd.RunPowerShell(currentPathToTestFrom, $"Test-Path -Path {pathToTest}");
    if (bufferedCmd.Result == null) { return -1; }
    return (bufferedCmd.Result.StandardOutput == "True\r\n") ? 1 : 0;
  }
}
