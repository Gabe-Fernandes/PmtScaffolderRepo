namespace PmtScaffolder;

public class UserInput
{
  private static readonly UserInput _instance = new();

  public string OverWrite = string.Empty;

  public string ProjPath { get; set; }
  public string TestProjPath { get; set; }
  public string ProjName { get; set; }
  public string TestProjName { get; set; }

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
