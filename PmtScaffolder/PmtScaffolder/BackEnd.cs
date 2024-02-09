using Humanizer;

namespace PmtScaffolder;

public static class BackEnd
{
  private static readonly UserInput _userInput = UserInput.GetUserInput();
  private static readonly string br = Environment.NewLine;

  public static async Task ScaffoldBackEndCode()
  {
    await PSCmd.RunPowerShellBatch(_userInput.ProjPath + "/Data", BackEndTemplates.AppDbCtx());
    Console.WriteLine(await GenerateCode(_userInput.ProjPath + "/Data/Models", "model class"));
    Console.WriteLine(await GenerateCode(_userInput.ProjPath + "/Data/RepoInterfaces", "repo interface"));
    Console.WriteLine(await GenerateCode(_userInput.ProjPath + "/Data/Repos", "repository"));
    Console.WriteLine(await GenerateCode(_userInput.TestProjPath + "/Data/Repos", "unit test"));
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
      List<string> mockData = [];
      List<string> dbCtxMockData = [];
      string model = _userInput.Models[i];

      for (int j = 0; j < _userInput.Properties[i].Count; j++)
      {
        string property = Util.Capital(_userInput.Properties[i][j], true);

        mockData.Add($"{br}\t\t{property} = {GetUnitTestMockData(_userInput.DataTypes[i][j])},");

        if (_userInput.Properties[i][j].ToLower() != "id") // Id is hardcoded in the template
        {
          dbCtxMockData.Add($"{br}\t\t\t\t\t{property} = {GetUnitTestMockData(_userInput.DataTypes[i][j])},");
        }
      }

      switch (fileType)
      {
        case "model class":
          await PSCmd.RunPowerShellBatch(filePath, BackEndTemplates.ModelClassHeader(model).Concat(GetProperties(i)).ToArray());
          if (model != "AppUser")
          {
            await Util.InsertCode(_userInput.ProjPath + "/Data", BackEndTemplates.DbSet(model, model.Pluralize()), "AppDbContext.cs");
          }
          break;
        case "repo interface":
          await PSCmd.RunPowerShellBatch(filePath, BackEndTemplates.RepoInterface(model));
          break;
        case "repository": 
          await PSCmd.RunPowerShellBatch(filePath, BackEndTemplates.Repository(model, model.Pluralize()));
          await Util.InsertWithCheck($"using {_userInput.ProjName}.Data.RepoInterfaces;{br}", _userInput.ProjPath, "program.cs", true);
          await Util.InsertWithCheck($"using {_userInput.ProjName}.Data.Models;{br}", _userInput.ProjPath, "program.cs", true);
          await Util.InsertWithCheck(BackEndTemplates.DiRepoService(model), _userInput.ProjPath, "program.cs");
          break;
        case "unit test":
          await PSCmd.RunPowerShellBatch(filePath, BackEndTemplates.UnitTest(model, model.Pluralize(), mockData.ToArray(), dbCtxMockData.ToArray()));
          break;
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

  private static List<string> GetProperties(int modelIndex)
  {
    List<string> props = [];

    for (int j = 0; j < _userInput.Properties[modelIndex].Count; j++)
    {
      // don't run this for the id property of AppUsers - if this is needed for all default AppUser props, the structure may change
      if (_userInput.Properties[modelIndex][j].ToLower() != "id" || _userInput.Models[modelIndex].ToLower() != "appuser")
      {
        props.Add($"{br}\tpublic {_userInput.DataTypes[modelIndex][j]} {Util.Capital(_userInput.Properties[modelIndex][j], true)} {{ get; set; }}");
      }
    }
    props.Add($"{br}}}'");
    props.Add($"> {Util.Capital(_userInput.Models[modelIndex], true)}.cs");

    return props;
  }

  //private static async Task CheckProgramCsForNamespaces(string model)
  //{
  //  string namespace0 = ;
  //  string namespace1 = ;
  //  string programCsText = await Util.ExtractFileText(_userInput.ProjPath, "program.cs");

  //  if (programCsText.Contains() == false)
  //  {
  //    await Util.InsertCode(_userInput.ProjPath, BackEndTemplates.DiRepoService(model), "program.cs");
  //  }
  //  if (programCsText.Contains(namespace0) == false)
  //  {
  //    string finalOutput = Util.PartitionCodeFile(namespace0 + programCsText);
  //    await PSCmd.RunPowerShellBatch(_userInput.ProjPath, Util.CreateTemplateFormat(finalOutput, "program.cs"));
  //  }
  //  if (programCsText.Contains(namespace1) == false)
  //  {
  //    string finalOutput = Util.PartitionCodeFile(namespace1 + programCsText);
  //    await PSCmd.RunPowerShellBatch(_userInput.ProjPath, Util.CreateTemplateFormat(finalOutput, "program.cs"));
  //  }
  //}
}
