namespace PmtScaffolder;

public class UserInput
{
  private static readonly UserInput _instance = new();

  public string ProjPath { get; set; } = "C:\\dev\\ps_scaffolding\\autogen\\autogen";
  public string TestProjPath { get; set; } = "C:\\dev\\ps_scaffolding\\autogen\\autogenTests";
  public string ProjName { get; set; } = "AutoGen";
  public string TestProjName { get; set; } = "AutoGenTests";

  public List<string> Controllers = ["Agile", "MyProjects"];
  public List<List<string>> FileNames = [["one", "two", "three"], ["four", "five", "six"]];

  public List<string> Models = ["Story", "Project"];
  public List<List<string>> DataTypes = [["int", "string", "string"], ["int", "string", "DateTime"]];
  public List<List<string>> Properties = [["Id", "Title", "Description"], ["Id", "Name", "DueDate"]];

  private UserInput() { }

  public static UserInput GetUserInput()
  {
    return _instance;
  }
}
