namespace PmtScaffolder;

public class UserInput
{
  private static readonly UserInput _instance = new();

  public string OverWrite = string.Empty;
  public List<string> ErrorReport = [];

  public string ProjPath { get; set; } = "C:\\dev\\PmtsRepo\\AutoGen\\AutoGen";
  public string TestProjPath { get; set; } = "C:\\dev\\PmtsRepo\\AutoGen\\AutoGenTests";
  public string ProjName { get; set; } = "AutoGen";
  public string TestProjName { get; set; } = "AutoGenTests";

  public List<string> Controllers = [];
  public List<List<string>> FileNames = [];

  public List<string> Models = [];
  public List<List<string>> DataTypes = [];
  public List<List<string>> Properties = [];

  private UserInput() { }

  public static UserInput GetUserInput()
  {
    return _instance;
  }
}
