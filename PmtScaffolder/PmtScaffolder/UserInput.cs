namespace PmtScaffolder;

public class UserInput
{
  private static readonly UserInput _instance = new();

  public string OverWrite = string.Empty;

  public string ProjPath { get; set; } = "C:\\dev\\ps_scaffolding\\autogen\\autogen";
  public string TestProjPath { get; set; } = "C:\\dev\\ps_scaffolding\\autogen\\autogenTests";
  public string ProjName { get; set; } = "AutoGen";
  public string TestProjName { get; set; } = "AutoGenTests";

  public List<string> Controllers = ["agile", "MyProjects"];
  public List<List<string>> FileNames = [["one", "Two", "three"], ["four", "five", "Six"]];

  public List<string> Models = ["story", "Project", "appUser"];
  public List<List<string>> DataTypes = [["int", "string", "string"], ["int", "string", "DateTime"], ["string", "string", "int", "DateTime"]];
  public List<List<string>> Properties = [["id", "Title", "Description"], ["Id", "name", "dueDate"], ["Id", "name", "age", "dob"]];

  private UserInput() { }

  public static UserInput GetUserInput()
  {
    return _instance;
  }
}
