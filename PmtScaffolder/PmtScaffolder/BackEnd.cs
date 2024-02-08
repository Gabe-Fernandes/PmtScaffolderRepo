﻿namespace PmtScaffolder;

public static class BackEnd
{
  private static readonly UserInput _userInput = UserInput.GetUserInput();
  private static readonly string br = Environment.NewLine;



  public static async Task ScaffoldBackEndCode()
  {
    Console.WriteLine(await GenerateCode(_userInput.ProjPath + "/Data", "db ctx"));
    Console.WriteLine(await GenerateCode(_userInput.ProjPath + "/Data/Models", "model class"));
    Console.WriteLine(await GenerateCode(_userInput.ProjPath + "/Data/RepoInterfaces", "repo interface"));
    Console.WriteLine(await GenerateCode(_userInput.ProjPath + "/Data/Repos", "repository"));
    Console.WriteLine(await GenerateCode(_userInput.TestProjPath + "/Data/Repos", "unit test"));
    // write unit tests for controllers
    // create test project (consider a 3rd area - front, back, tests)
    // (insertion) add db sets to dbCtx
    // create dbctx
    // AppUser edge case with unit tests
  }

  private static async Task<string> GenerateCode(string filePath, string fileType, bool overwrite = true)
  {
    switch (Util.TestPath(_userInput.ProjPath, filePath))
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
      string model = _userInput.Models[i];

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
      props.Add($"> {model}.cs");

      switch (fileType)
      {
        case "model class": await PSCmd.RunPowerShellBatch(filePath, BackEndTemplates.ModelClassHeader(model).Concat(props).ToArray()); break;
        case "repo interface": await PSCmd.RunPowerShellBatch(filePath, BackEndTemplates.RepoInterface(model)); break;
        case "repository": await PSCmd.RunPowerShellBatch(filePath, BackEndTemplates.Repository(model));
                           await Util.InsertCode(_userInput.ProjPath, BackEndTemplates.DiRepoService(model), "program.cs");
                           await CheckProgramCsForNamespaces(); break;
        case "unit test": await PSCmd.RunPowerShellBatch(filePath, BackEndTemplates.UnitTest(model, mockData.ToArray(), dbCtxMockData.ToArray())); break;
        case "db ctx": await PSCmd.RunPowerShellBatch(filePath, BackEndTemplates.AppDbCtx()); break;
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

  private static async Task CheckProgramCsForNamespaces()
  {
    string namespace0 = $"using {_userInput.ProjName}.Data.RepoInterfaces;{br}";
    string namespace1 = $"using {_userInput.ProjName}.Data.Models;{br}";
    string programCsText = await Util.ExtractFileText(_userInput.ProjPath, "program.cs");

    if (programCsText.Contains(namespace0) == false)
    {
      string finalOutput = Util.PartitionCodeFile(namespace0 +  programCsText);
      await PSCmd.RunPowerShellBatch(_userInput.ProjPath, Util.CreateTemplateFormat(finalOutput, "program.cs"));
    }
    if (programCsText.Contains(namespace1) == false)
    {
      string finalOutput = Util.PartitionCodeFile(namespace1 + programCsText);
      await PSCmd.RunPowerShellBatch(_userInput.ProjPath, Util.CreateTemplateFormat(finalOutput, "program.cs"));
    }
  }
}
