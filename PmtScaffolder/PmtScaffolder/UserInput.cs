namespace PmtScaffolder;

public class UserInput
{
  private static readonly UserInput _instance = new();

  public string ProjPath { get; set; }
  public string ProjName { get; set; }
  public List<string> Models = [];
  public List<string> FileNames = [];
  public List<string> DataTypes = [];
  public List<string> Properties = [];
  public List<string> Controllers = [];

  private UserInput() { }

  public static UserInput GetUserInput()
  {
    return _instance;
  }
}
